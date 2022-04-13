using System;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertsByPersonId
    {
        Task<CautionaryAlertsMMHPersonResponse> ExecuteAsync(Guid personId);
    }
}
