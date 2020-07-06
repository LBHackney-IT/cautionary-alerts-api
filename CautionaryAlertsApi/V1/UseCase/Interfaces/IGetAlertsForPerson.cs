using CautionaryAlertsApi.V1.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetAlertsForPerson
    {
        ListPersonsCautionaryAlerts Execute(string tagRef, string personNo);
    }
}
