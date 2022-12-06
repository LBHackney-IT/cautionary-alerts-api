using System.Linq;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Factories;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetGoogleSheetAlertsForPerson : IGetGoogleSheetAlertsForPerson
    {
        private readonly IGoogleSheetGateway _gateway;

        public GetGoogleSheetAlertsForPerson(IGoogleSheetGateway gateway)
        {
            _gateway = gateway;
        }

        public CautionaryAlertsGoogleSheetPersonResponse Execute(string personId)
        {
            var result = _gateway.GetPersonAlerts(personId).ToList();
            if (result.Count == 0) throw new PersonNotFoundException();

            return new CautionaryAlertsGoogleSheetPersonResponse
            {
                PersonId = personId,
                Alerts = result.Select(alert => alert.ToCautionaryAlertGoogleSheetResponse()).ToList()
            };
        }
    }
}
