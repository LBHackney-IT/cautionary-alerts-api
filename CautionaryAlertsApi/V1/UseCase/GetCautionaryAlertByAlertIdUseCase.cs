using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Hackney.Shared.CautionaryAlerts.Domain;
using CautionaryAlertsApi.V1.Boundary.Request;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryAlertByAlertIdUseCase : IGetCautionaryAlertByAlertIdUseCase
    {
        private readonly IUhGateway _gateway;

        public GetCautionaryAlertByAlertIdUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public PropertyAlertDomain ExecuteAsync(AlertQueryObject query)
        {
            var result = _gateway.GetCautionaryAlertByAlertId(query);

            return result;
        }
    }
}
