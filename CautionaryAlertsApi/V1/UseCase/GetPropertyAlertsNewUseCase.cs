using System.Linq;
using System.Threading.Tasks;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetPropertyAlertsNewUseCase : IPropertyAlertsNewUseCase
    {
        private readonly IUhGateway _gateway;

        public GetPropertyAlertsNewUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<CautionaryAlertsPropertyResponse> ExecuteAsync(string propertyReference)
        {
            var result = await _gateway.GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            return new CautionaryAlertsPropertyResponse
            {
                PropertyReference = propertyReference,
                Alerts = result.Select(r => r.ToResponse()).ToList()
            };
        }
    }
}
