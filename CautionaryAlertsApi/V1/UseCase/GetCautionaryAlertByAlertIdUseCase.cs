using CautionaryAlertsApi.V1.Gateways;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using System.Threading.Tasks;
using System;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Hackney.Shared.CautionaryAlerts.Factories;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryAlertByAlertIdUseCase : IGetCautionaryAlertByAlertIdUseCase
    {
        private readonly IUhGateway _gateway;

        public GetCautionaryAlertByAlertIdUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public CautionaryAlertResponse ExecuteAsync(Guid personId, Guid alertId)
        {
            var result = _gateway.GetCautionaryAlertByAlertId(personId, alertId);

            return result.ToResponse();
        }
    }
}
