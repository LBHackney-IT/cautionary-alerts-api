using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

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
