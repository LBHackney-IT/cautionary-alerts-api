using System.Collections.Generic;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Infrastructure;

namespace CautionaryAlertsApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class ExampleGateway : IExampleGateway
    {
        private readonly UhContext _uhContext;

        public ExampleGateway(UhContext uhContext)
        {
            _uhContext = uhContext;
        }

        public CautionaryAlert GetEntityById(int id)
        {
            var result = _uhContext.PeopleAlerts.Find(id);

            return result?.ToDomain();
        }

        public List<CautionaryAlert> GetAll()
        {
            return new List<CautionaryAlert>();
        }
    }
}
