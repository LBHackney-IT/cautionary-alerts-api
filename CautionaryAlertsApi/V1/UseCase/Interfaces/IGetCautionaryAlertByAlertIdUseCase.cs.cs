using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertByAlertIdUseCase
    {
        PropertyAlertDomain ExecuteAsync(AlertQueryObject query);
    }
}
