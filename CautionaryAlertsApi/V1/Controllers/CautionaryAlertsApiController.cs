using CautionaryAlertsApi.V1.Boundary.Response;
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
        public CautionaryAlertsApiController(IGetAlertsForPerson getAlertsForPerson)
        {
            _getAlertsForPerson = getAlertsForPerson;
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
    }
}
