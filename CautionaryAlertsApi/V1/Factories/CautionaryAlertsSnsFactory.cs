using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;
using Hackney.Core.JWT;
using Hackney.Shared.CautionaryAlerts.Domain;
using System;

namespace CautionaryAlertsApi.V1.Factories
{
    public class CautionaryAlertsSnsFactory : ISnsFactory
    {
        public CautionaryAlertSns Create(PropertyAlertDomain alert, Token token)
        {
            return new CautionaryAlertSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = Guid.Parse(alert.MMHID),
                Id = Guid.NewGuid(),
                EventType = Constants.CREATED_EVENTTYPE,
                Version = Constants.V1_VERSION,
                SourceDomain = Constants.SOURCE_DOMAIN,
                SourceSystem = Constants.SOURCE_SYSTEM,
                User = new User
                {
                    Name = token.Name,
                    Email = token.Email
                },
                EventData = new EventData
                {
                    NewData = alert
                }
            };
        }

        public CautionaryAlertSns End(PropertyAlertDomain alert, Token token)
        {
            return new CautionaryAlertSns
            {
                CorrelationId = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                EntityId = Guid.Parse(alert.MMHID) /*change to alertId*/,
                Id = Guid.NewGuid(),
                EventType = Constants.ENDED_EVENTTYPE,
                Version = Constants.V1_VERSION,
                SourceDomain = Constants.SOURCE_DOMAIN,
                SourceSystem = Constants.SOURCE_SYSTEM,
                User = new User
                {
                    Name = token.Name,
                    Email = token.Email
                },
                EventData = new EventData
                {
                    NewData = alert
                }
            };
        }
    }
}
