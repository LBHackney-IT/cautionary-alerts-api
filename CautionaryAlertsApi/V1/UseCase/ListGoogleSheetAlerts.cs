using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class ListGoogleSheetAlerts : IListGoogleSheetAlerts
    {
        private readonly IGoogleSheetGateway _gateway;

        public ListGoogleSheetAlerts(IGoogleSheetGateway gateway)
        {
            _gateway = gateway;
        }

        public IEnumerable<CautionaryAlertListItem> Execute()
        {
            return _gateway.ListPropertyAlerts().ToList();
        }
    }
}
