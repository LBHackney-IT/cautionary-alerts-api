using System;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.UseCase;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CautionaryAlertsApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/sheet/cautionary-alerts")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class GoogleSheetAlertsController : BaseController
    {
        private readonly IGetGoogleSheetAlertsForProperty _getAlertsForProperty;

        public GoogleSheetAlertsController(IGetGoogleSheetAlertsForProperty getAlertsForProperty)
        {
            _getAlertsForProperty = getAlertsForProperty;
        }

        /// <summary>
        /// Returns a list of Google Sheet cautionary alerts for a property based on property reference
        /// </summary>
        /// <param name="propertyReference">The housing property reference of a property</param>
        /// <response code="200">Successful. Returns one or more cautionary alerts for a property.</response>
        /// <response code="404">No property cautionary alerts found for this property reference</response>
        [ProducesResponseType(typeof(CautionaryAlertsPropertyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    }
}
