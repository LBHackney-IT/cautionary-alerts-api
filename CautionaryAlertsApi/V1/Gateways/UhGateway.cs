using System.Collections.Generic;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Infrastructure;

namespace CautionaryAlertsApi.V1.Gateways
{
    public class UhGateway : IUhGateway
    {
        private readonly UhContext _uhContext;

        public UhGateway(UhContext uhContext)
        {
            _uhContext = uhContext;
        }

        public List<CautionaryAlertPerson> GetCautionaryAlertsForAPerson(string tagRef, string personNumber)
        {

            return new List<CautionaryAlertPerson>();
        }
    }
}
