using System.Collections.Generic;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.Gateways
{
    public interface IExampleGateway
    {
        CautionaryAlert GetEntityById(int id);

        List<CautionaryAlert> GetAll();
    }
}
