using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;
using Hackney.Core.JWT;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IPostNewCautionaryAlertUseCase
    {
        Task<CautionaryAlertListItem> ExecuteAsync(CreateCautionaryAlert createCautionaryAlert, Token token);
    }
}
