using System.Collections.Generic;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.V1.UseCase
{
    public interface IListGoogleSheetAlerts
    {
        IEnumerable<CautionaryAlertListItem> Execute();
    }
}
