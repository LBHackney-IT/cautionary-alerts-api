using System;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.UseCase;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CautionaryAlertsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/cautionary-alerts")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class CautionaryAlertsApiController : BaseController
    {
        private readonly IGetAlertsForPeople _getAlertsForPeople;
        private readonly IGetCautionaryAlertsForProperty _getCautionaryAlertsForProperty;
        private readonly IPropertyAlertsNewUseCase _getPropertyAlertsNewUseCase;
        private readonly IGetCautionaryAlertsByPersonId _getCautionaryAlertsByPersonId;
        private readonly IPostNewCautionaryAlertUseCase _postNewCautionaryAlertUseCase;

        public CautionaryAlertsApiController(IGetAlertsForPeople getAlertsForPeople,
                                             IGetCautionaryAlertsForProperty getCautionaryAlertsForProperty,
                                             IPropertyAlertsNewUseCase getCautionaryContactAlertsUseCase,
                                             IGetCautionaryAlertsByPersonId getCautionaryAlertsByPersonId,
                                             IPostNewCautionaryAlertUseCase postNewCautionaryAlertUseCase)
        {
            _getAlertsForPeople = getAlertsForPeople;
            _getCautionaryAlertsForProperty = getCautionaryAlertsForProperty;
            _getPropertyAlertsNewUseCase = getCautionaryContactAlertsUseCase;
            _getCautionaryAlertsByPersonId = getCautionaryAlertsByPersonId;
            _postNewCautionaryAlertUseCase = postNewCautionaryAlertUseCase;
        }

        /// <summary>
        /// Retrieve cautionary alerts for a person
        /// </summary>
        /// <param name="tagRef">The Tenancy Reference for the person you are interested in. e.g. 1265372/01</param>
        /// <param name="personNo">The number of the person you are interested in within a property. person_no in UH. e.g. 01</param>
        /// <response code="200">Successful. One or more records found for given tag_ref and person_no</response>
        /// <response code="404">No records found for given tag_ref and person_no</response>
        [ProducesResponseType(typeof(ListPersonsCautionaryAlerts), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("people")]
        public IActionResult ViewPeopleCautionaryAlerts([FromQuery(Name = "tag_ref"), BindRequired] string tagRef,
            [FromQuery(Name = "person_number")] string personNo)
        {
            try
            {
                return Ok(_getAlertsForPeople.Execute(tagRef, personNo));
            }
            catch (PersonNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Returns a list of cautionary alerts for a property based on property reference
        /// </summary>
        /// <param name="propertyReference">The housing property reference of a property</param>
        /// <response code="200">Successful. Returns one or more cautionary alerts for a property.</response>
        /// <response code="404">No property cautionary alerts found for this property reference</response>
        [ProducesResponseType(typeof(CautionaryAlert), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("properties/{propertyReference}")]
        public IActionResult ViewPropertyCautionaryAlerts(string propertyReference)
        {
            try
            {
                return Ok(_getCautionaryAlertsForProperty.Execute(propertyReference));
            }
            catch (PropertyAlertNotFoundException)
            {
                return NotFound($"Property cautionary alert(s) for property reference {propertyReference} not found");
            }
        }

        /// <summary>
        /// Returns a list of cautionary alerts for a property based on property reference.
        /// Reads from new table in database to mitigate GS performance issues
        /// </summary>
        /// <param name="propertyReference">The housing property reference of a property</param>
        /// <response code="200">Successful. Returns one or more cautionary alerts for a property.</response>
        [ProducesResponseType(typeof(CautionaryAlertsPropertyResponse), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("properties-new/{propertyReference}")]
        public async Task<IActionResult> GetPropertyAlertsNew(string propertyReference)
        {
            var result = await _getPropertyAlertsNewUseCase.ExecuteAsync(propertyReference).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// Returns a list of cautionary alerts based on person ID.
        /// Reads from new table in database to mitigate GS performance issues
        /// </summary>
        /// <param name="personId">A unique MMH identifier (GUID) of a person.</param>
        /// <response code="200">Successful. Returns a list of cautionary alerts for a person.</response>
        [ProducesResponseType(typeof(CautionaryAlertsPropertyResponse), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("persons/{personId}")]
        public async Task<IActionResult> GetAlertsByPersonId([FromRoute] Guid personId)
        {
            var result = await _getCautionaryAlertsByPersonId.ExecuteAsync(personId).ConfigureAwait(false);

            return Ok(result);
        }

        [ProducesResponseType(typeof(CautionaryAlertResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> CreateNewCautionaryAlert([FromBody] CreateCautionaryAlert cautionaryAlert)
        {
            var result = await _postNewCautionaryAlertUseCase.Execute(cautionaryAlert).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
