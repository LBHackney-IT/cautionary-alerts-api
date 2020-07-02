using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

namespace CautionaryAlertsApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetClaimantByIdUseCase
    public class GetByIdUseCase : IGetByIdUseCase
    {
        private IUhGateway _gateway;
        public GetByIdUseCase(IUhGateway gateway)
        {
            _gateway = gateway;
        }

        //TODO: rename id to the name of the identifier that will be used for this API, the type may also need to change
        public CautionaryAlertResponse Execute(int id)
        {
            return _gateway.GetEntityById(id).ToResponse();
        }
    }
}
