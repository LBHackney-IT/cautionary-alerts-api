using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class PostNewCautionaryAlertUseCase : IPostNewCautionaryAlertUseCase
    {
        private readonly IUhGateway _gateway;

        public PostNewCautionaryAlertUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<CautionaryAlertListItem> ExecuteAsync(CreateCautionaryAlert cautionaryAlert)
        {
            var result = await _gateway.PostNewCautionaryAlert(cautionaryAlert).ConfigureAwait(false);
            return result;
        }
    }
}
