using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Domain;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using AlertQueryObject = CautionaryAlertsApi.V1.Boundary.Request.AlertQueryObject;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IUhGateway
    {
        List<CautionaryAlertPerson> GetCautionaryAlertsForPeople(string tagRef, string personNumber);

        CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetPropertyAlertsNew(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetCautionaryAlertsByMMHPersonId(Guid personId);

        PropertyAlertDomain GetCautionaryAlertByAlertId(AlertQueryObject query);
        Task<PropertyAlertDomain> PostNewCautionaryAlert(CreateCautionaryAlert cautionaryAlert);

        Task<PropertyAlertDomain> EndCautionaryAlert(AlertQueryObject updateAlert, EndCautionaryAlertRequest endCautionaryAlertRequest);
    }
}
