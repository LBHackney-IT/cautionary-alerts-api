using System;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CautionaryAlertsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/cautionary-alerts")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class CautionaryAlertsApiController : BaseController
    {
        private readonly IGetAlertsForPerson _getAlertsForPerson;
        public CautionaryAlertsApiController(IGetAlertsForPerson getAlertsForPerson)
        {
            _getAlertsForPerson = getAlertsForPerson;
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
        public IActionResult ViewPersonsCautionaryAlerts([FromQuery(Name = "tag-ref"), BindRequired] string tagRef,
            [FromQuery(Name = "person-number"), BindRequired] string personNo)
        {
            try
            {
                return Ok(_getAlertsForPerson.Execute(tagRef, personNo));
            }
            catch (PersonNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
