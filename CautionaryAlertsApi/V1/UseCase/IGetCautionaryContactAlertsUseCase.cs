using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase
{
    public interface IGetCautionaryContactAlertsUseCase
    {
        Task<CautionaryAlertsPropertyResponse> ExecuteAsync(string propertyReference);
    }
}
