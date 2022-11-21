using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Gateways;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IPostNewCautionaryAlertUseCase
    {
        Task<CautionaryAlertListItem> Execute(CreateCautionaryAlert cautionaryAlert);
    }
}
