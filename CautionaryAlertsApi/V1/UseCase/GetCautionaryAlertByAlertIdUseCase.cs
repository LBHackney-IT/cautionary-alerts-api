using CautionaryAlertsApi.V1.Gateways;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using System.Threading.Tasks;
using System;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryAlertByAlertIdUseCase : IGetCautionaryAlertByAlertIdUseCase
    {
        private readonly IUhGateway _gateway;

        public GetCautionaryAlertByAlertIdUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public CautionaryAlert ExecuteAsync(AlertQueryObject query)
        {
            var result = _gateway.GetCautionaryAlertByAlertId(query);

            return result;
        }
    }
}
