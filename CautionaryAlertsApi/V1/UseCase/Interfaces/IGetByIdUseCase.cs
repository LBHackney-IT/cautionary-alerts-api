using CautionaryAlertsApi.V1.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        CautionaryAlertResponse Execute(int id);
    }
}
