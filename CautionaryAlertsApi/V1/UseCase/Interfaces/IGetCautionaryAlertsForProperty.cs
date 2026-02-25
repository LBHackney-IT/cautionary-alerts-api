using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetCautionaryAlertsForProperty
    {
        CautionaryAlertsPropertyResponse Execute(string propertyReference);
    }
}
