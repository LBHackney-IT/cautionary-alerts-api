using System;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.UseCase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CautionaryAlertsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/cautionary-alerts/sheets")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class GoogleSheetAlertsController : BaseController
    {
        private readonly IGetGoogleSheetAlertsForProperty _getAlertsForProperty;
        private readonly IGetGoogleSheetAlertsForPerson _getAlertsForPerson;

        public GoogleSheetAlertsController(
            IGetGoogleSheetAlertsForProperty getAlertsForProperty,
            IGetGoogleSheetAlertsForPerson getAlertsForPerson)
        {
            _getAlertsForProperty = getAlertsForProperty;
            _getAlertsForPerson = getAlertsForPerson;
        }

        /// <summary>
        /// Returns a list of Google Sheet cautionary alerts for a property based on property reference
        /// </summary>
        /// <param name="propertyReference">The housing property reference of a property</param>
        /// <response code="200">Successful. Returns one or more cautionary alerts for a property.</response>
        /// <response code="404">No property cautionary alerts found for this property reference</response>
        [ProducesResponseType(typeof(CautionaryAlertsPropertyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("properties/{propertyReference}")]
        public IActionResult GetAlertsByProperty(string propertyReference)
        {
            try
            {
                return Ok(_getAlertsForProperty.Execute(propertyReference));
            }
            catch (PropertyAlertNotFoundException)
            {
                return NotFound($"Cautionary alert(s) for property reference {propertyReference} not found");
            }
        }

        /// <summary>
        /// Returns a list of discretion alerts for a person with the given <paramref name="personId"/> from a Google Sheet
        /// </summary>
        /// <param name="personId">A unique identifier of a person to get discretion alerts for.</param>
        /// <response code="200">Successful. Returns one or more discretion alerts for a person.</response>
        /// <response code="404">No discretion alerts found for this person ID</response>
        [ProducesResponseType(typeof(DiscretionAlertsPersonResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("persons/{personId}")]
        public IActionResult GetAlertsByPerson(string personId)
        {
            try
            {
                return Ok(_getAlertsForPerson.Execute(personId));
            }
            catch (PropertyAlertNotFoundException)
            {
                return NotFound($"Discretion alert(s) for person ID {personId} not found");
            }
        }
    }
}
