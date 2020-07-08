using System.Collections.Generic;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IUhGateway
    {
        List<CautionaryAlertPerson> GetCautionaryAlertsForAPerson(string tagRef, string personNumber);
        CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference);
    }
}
