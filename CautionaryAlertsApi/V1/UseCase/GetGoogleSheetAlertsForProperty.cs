using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<CautionaryAlertListItem> Execute(string propertyReference)
        {
            return _gateway.GetPropertyAlerts(propertyReference).ToList();
        }
    }
}
