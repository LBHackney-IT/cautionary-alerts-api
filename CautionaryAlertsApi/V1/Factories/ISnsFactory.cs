using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;
using Hackney.Core.JWT;

namespace CautionaryAlertsApi.V1.Factories
{
    public interface ISnsFactory
    {
        CautionaryAlertSns Create(PropertyAlertDomain alert, Token token);
    }
}
