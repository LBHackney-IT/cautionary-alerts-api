using System;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryAlertsByPersonIdUseCase : IGetCautionaryAlertsByPersonId
    {
        private readonly IUhGateway _gateway;

        public GetCautionaryAlertsByPersonIdUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<CautionaryAlertsMMHPersonResponse> ExecuteAsync(Guid personId)
        {
            var result = await _gateway.GetCautionaryAlertsByMMHPersonId(personId).ConfigureAwait(false);

            return new CautionaryAlertsMMHPersonResponse
            {
                PersonId = personId,
                Alerts = result.Select(r => r.ToResponse()).ToList()
            };
        }
    }
}
