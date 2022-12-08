using System.Collections.Generic;
using CautionaryAlertListItem = Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets.CautionaryAlertListItem;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IGoogleSheetGateway
    {
        IEnumerable<CautionaryAlertListItem> GetPropertyAlerts(string propertyReference);

        IEnumerable<CautionaryAlertListItem> GetPersonAlerts(string personId);
    }
}
