using System;
using CautionaryAlertsApi.V1.Domain;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;

namespace CautionaryAlertsApi.V1.Factories
{
    public static class PresentationDomainFactory
    {
        public static EndCautionaryAlert ToDomain(this AlertQueryObject presentationObj)
        {
            if (presentationObj is null)
                throw new ArgumentNullException(nameof(presentationObj));

            return new EndCautionaryAlert(presentationObj.AlertId);
        }
    }
}
