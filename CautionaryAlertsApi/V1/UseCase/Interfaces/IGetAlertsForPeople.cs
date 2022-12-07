using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetAlertsForPeople
    {
        ListPersonsCautionaryAlerts Execute(string tagRef, string personNo);
    }
}
