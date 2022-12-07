using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IPostNewCautionaryAlertUseCase
    {
        Task<CautionaryAlertListItem> ExecuteAsync(CreateCautionaryAlert createCautionaryAlert, Token token);
    }
}
