using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase
{
    public interface IGetGoogleSheetAlertsForPerson
    {
        CautionaryAlertsGoogleSheetPersonResponse Execute(string personId);
    }
}
