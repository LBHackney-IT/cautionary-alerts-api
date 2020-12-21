
namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IValidatePropertyReference
    {
        bool Execute(string postcode);
    }
}
