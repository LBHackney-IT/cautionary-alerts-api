using CautionaryAlertsApi.V1.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase
{
    public interface IGetGoogleSheetAlertsForPerson
    {
        CautionaryAlertsGoogleSheetPersonResponse Execute(string personId);
    }
}