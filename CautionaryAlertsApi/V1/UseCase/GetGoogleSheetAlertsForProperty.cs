using System.Linq;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetGoogleSheetAlertsForProperty : IGetGoogleSheetAlertsForProperty
    {
        private readonly IGoogleSheetGateway _gateway;

        public GetGoogleSheetAlertsForProperty(IGoogleSheetGateway gateway)
        {
            _gateway = gateway;
        }

        public CautionaryAlertsPropertyResponse Execute(string propertyReference)
        {
            var result = _gateway.GetPropertyAlerts(propertyReference).ToList();
            if (result.Count == 0) throw new PropertyAlertNotFoundException();

            return new CautionaryAlertsPropertyResponse
            {
                PropertyReference = propertyReference,
                Alerts = result.Select(r => r.ToResponse()).ToList()
            };
        }
    }
}
