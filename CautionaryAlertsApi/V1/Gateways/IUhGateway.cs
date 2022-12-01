using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IUhGateway
    {
        List<CautionaryAlertPerson> GetCautionaryAlertsForPeople(string tagRef, string personNumber);

        CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetPropertyAlertsNew(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetCautionaryAlertsByMMHPersonId(Guid personId);

        Task<CautionaryAlertListItem> PostNewCautionaryAlert(CreateCautionaryAlert cautionaryAlert);
    }
}
