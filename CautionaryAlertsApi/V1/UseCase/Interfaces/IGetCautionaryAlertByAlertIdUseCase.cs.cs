using CautionaryAlertsApi.V1.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertByAlertIdUseCase
    {
        PropertyAlertDomain ExecuteAsync(AlertQueryObject query);
    }
}
