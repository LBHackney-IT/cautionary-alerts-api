using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Request;
using Hackney.Core.JWT;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IEndCautionaryAlertUseCase
    {
        Task<PropertyAlertDomain> ExecuteAsync(AlertQueryObject query, EndCautionaryAlertRequest endCautionaryAlertRequest, Token token);
    }
}
