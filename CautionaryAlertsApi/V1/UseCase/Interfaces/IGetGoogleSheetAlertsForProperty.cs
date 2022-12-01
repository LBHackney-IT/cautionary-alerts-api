using System.Collections.Generic;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetGoogleSheetAlertsForProperty
    {
        CautionaryAlertsPropertyResponse Execute(string propertyReference);
    }
}
