using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase
{
    public interface IPropertyAlertsNewUseCase
    {
        Task<CautionaryAlertsPropertyResponse> ExecuteAsync(string propertyReference);
    }
}
