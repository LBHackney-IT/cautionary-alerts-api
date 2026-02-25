using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetGoogleSheetAlertsForProperty
    {
        CautionaryAlertsPropertyResponse Execute(string propertyReference);
    }
}
