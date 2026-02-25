using CautionaryAlertsApi.V1.Domain;
using Hackney.Core.JWT;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.V1.Factories
{
    public interface ISnsFactory
    {
        CautionaryAlertSns Create(PropertyAlertDomain alert, Token token);

        CautionaryAlertSns End(PropertyAlertDomain alert, Token token);
    }
}
