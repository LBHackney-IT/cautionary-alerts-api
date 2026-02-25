using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryAlertsForProperty : IGetCautionaryAlertsForProperty
    {
        private IUhGateway _gateway;
        public GetCautionaryAlertsForProperty(IUhGateway gateway)
        {
            _gateway = gateway;
        }
        public CautionaryAlertsPropertyResponse Execute(string propertyReference)
        {
            var response = _gateway.GetCautionaryAlertsForAProperty(propertyReference);

            if (response == null)
            {
                throw new PropertyAlertNotFoundException();
            }
            return ResponseFactory.ToResponse(response);
        }
    }
}
