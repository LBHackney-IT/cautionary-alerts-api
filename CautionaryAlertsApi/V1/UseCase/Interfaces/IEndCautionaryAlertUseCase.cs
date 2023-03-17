using System.Threading.Tasks;
using Hackney.Core.JWT;
using Hackney.Shared.CautionaryAlerts.Domain;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IEndCautionaryAlertUseCase
    {
        Task<PropertyAlertDomain> ExecuteAsync(EndCautionaryAlert query, Token token);
    }
}
