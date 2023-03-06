using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Domain;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IUhGateway
    {
        List<CautionaryAlertPerson> GetCautionaryAlertsForPeople(string tagRef, string personNumber);

        CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetPropertyAlertsNew(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetCautionaryAlertsByMMHPersonId(Guid personId);

        CautionaryAlert GetCautionaryAlertByAlertId(Guid personId, Guid alertId);
        Task<PropertyAlertDomain> PostNewCautionaryAlert(CreateCautionaryAlert cautionaryAlert);

    }
}
