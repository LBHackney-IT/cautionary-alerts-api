using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;
using System.Collections.Generic;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IGoogleSheetGateway
    {
        IEnumerable<CautionaryAlertListItem> GetPropertyAlerts(string propertyReference);

        IEnumerable<CautionaryAlertListItem> GetPersonAlerts(string personId);
    }
}
