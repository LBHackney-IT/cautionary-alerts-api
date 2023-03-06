using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using System.Threading.Tasks;
using System;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertByAlertIdUseCase
    {
        CautionaryAlertResponse ExecuteAsync(Guid personId, Guid alertId);
    }
}
