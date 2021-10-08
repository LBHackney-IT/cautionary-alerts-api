using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.Infrastructure;
using PropertyAlert = CautionaryAlertsApi.V1.Infrastructure.PropertyAlert;

namespace CautionaryAlertsApi.V1.Factories
{
    public static class EntityFactory
    {
        public static CautionaryAlert ToDomain(this PersonAlert alert, string description)
        {
            return new CautionaryAlert
            {
                Description = description,
                AlertCode = alert.AlertCode,
                DateModified = alert.DateModified,
                EndDate = alert.EndDate,
                StartDate = alert.StartDate,
                ModifiedBy = alert.ModifiedBy
            };
        }
        public static CautionaryAlert ToDomain(this PropertyAlert alert, string description)
        {
            return new CautionaryAlert
            {
                Description = description,
                AlertCode = alert.AlertCode,
                DateModified = alert.DateModified,
                EndDate = alert.EndDate,
                StartDate = alert.StartDate,
                ModifiedBy = alert.ModifiedBy
            };
        }

        public static CautionaryAlertListItem ToModel(this IEnumerable<string> row)
        {
            var rowArray = row.ToArray();
            return new CautionaryAlertListItem
            {
                Name = rowArray[0],
                DoorNumber = rowArray[1],
                Address = rowArray[2],
                Neighbourhood = rowArray[3],
                DateOfIncident = rowArray[4],
                NumberOfDaysOutstanding = rowArray[5],
                Code = rowArray[6],
                LetterSent = rowArray[7],
                OnCivica = rowArray[8],
                Outcome = rowArray[9],
                CautionOnSystem = rowArray[10],
                ActionOnAssure = rowArray[11],
                Lookup = rowArray[12],
                PropertyReference = rowArray[13],
                TenancyDates = rowArray[14],
                IncidentBeforeCurrentTenancyDate = rowArray[15]
            };
        }
    }
}
