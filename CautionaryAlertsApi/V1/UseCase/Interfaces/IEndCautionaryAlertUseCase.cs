using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Core.JWT;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IEndCautionaryAlertUseCase
    {
        Task<PropertyAlertDomain> ExecuteAsync(AlertQueryObject query, Token token);
    }
}
