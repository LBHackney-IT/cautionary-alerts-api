using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Gateways;

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
