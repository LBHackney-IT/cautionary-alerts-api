using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetAlertsForPerson : IGetAlertsForPerson
    {
        private IUhGateway _gateway;
        public GetAlertsForPerson(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public CautionaryAlertPersonResponse Execute(string tagRef, string personNo)
        {
            return new CautionaryAlertPersonResponse();
        }
    }
}
