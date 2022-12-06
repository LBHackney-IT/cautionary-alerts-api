using System;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertsByPersonId
    {
        Task<CautionaryAlertsMMHPersonResponse> ExecuteAsync(Guid personId);
    }
}
