using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class PostNewCautionaryAlertUseCase : IPostNewCautionaryAlertUseCase
    {
        private readonly IUhGateway _gateway;

        public PostNewCautionaryAlertUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<CautionaryAlertListItem> Execute(CreateCautionaryAlert cautionaryAlert)
        {
            var result = await _gateway.PostNewCautionaryAlert(cautionaryAlert).ConfigureAwait(false);
            return result;
        }
    }
}
