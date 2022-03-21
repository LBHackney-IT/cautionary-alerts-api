using System.Linq;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryContactAlertsUseCase : IGetCautionaryContactAlertsUseCase
    {
        private readonly IUhGateway _gateway;

        public GetCautionaryContactAlertsUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<CautionaryAlertsPropertyResponse> ExecuteAsync(string propertyReference)
        {
            var result = await _gateway.GetCautionaryContacts(propertyReference).ConfigureAwait(false);

            return new CautionaryAlertsPropertyResponse
            {
                PropertyReference = propertyReference,
                Alerts = result.Select(r => r.ToResponse()).ToList()
            };
        }
    }
}
