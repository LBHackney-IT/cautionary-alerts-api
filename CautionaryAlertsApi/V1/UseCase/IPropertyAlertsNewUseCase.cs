using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase
{
    public interface IPropertyAlertsNewUseCase
    {
        Task<CautionaryAlertsPropertyResponse> ExecuteAsync(string propertyReference);
    }
}
