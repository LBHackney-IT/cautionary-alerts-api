using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CautionaryAlertsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/cautionary-alerts")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class CautionaryAlertsApiController : BaseController
    {
        private readonly IGetAlertsForPerson _getAlertsForPerson;
        private readonly IGetCautionaryAlertsForProperty _getCautionaryAlertsForProperty;
        public CautionaryAlertsApiController(IGetAlertsForPerson getAlertsForPerson, IGetCautionaryAlertsForProperty getCautionaryAlertsForProperty)
        {
            _getAlertsForPerson = getAlertsForPerson;
            _getCautionaryAlertsForProperty = getCautionaryAlertsForProperty;
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="404">No ? found for the specified ID</response>
        [ProducesResponseType(typeof(CautionaryAlertResponse), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("tag-ref/{tagRef}/person-number/{personNo}")]
        public IActionResult ViewPersonsCautionaryAlerts(string tagRef, string personNo)
        {
            return Ok(_getAlertsForPerson.Execute(tagRef, personNo));
        }

        /// <summary>
        /// Returns a list of cautionary alerts for a property based on property reference
        /// </summary>
        /// <param name="propertyReference">The housing property reference of a property</param>
        /// <response code="200">Successful. Returns one or more cautionary alerts for a property.</response>
        /// <response code="404">No property cautionary alerts found for this property reference</response>
        [ProducesResponseType(typeof(CautionaryAlertsPropertyResponse), StatusCodes.Status200OK)]
        [HttpGet]
        [Route("propertyReference/{propertyReference}")]
        public IActionResult ViewPropertyCautionaryAlerts(string propertyReference)
        {
            try
            {
                return Ok(_getCautionaryAlertsForProperty.Execute(propertyReference));
            }
            catch(PropertyAlertNotFoundException)
            {
                return NotFound($"Property cautionary alert(s) for property reference {propertyReference} not found");
            }
        }
    }
}
