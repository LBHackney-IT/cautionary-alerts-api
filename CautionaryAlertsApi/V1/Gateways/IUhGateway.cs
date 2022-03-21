using System.Collections.Generic;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IUhGateway
    {
        List<CautionaryAlertPerson> GetCautionaryAlertsForPeople(string tagRef, string personNumber);

        CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference);

        Task<IEnumerable<CautionaryAlertListItem>> GetPropertyAlertsNew(string propertyReference);

    }
}
