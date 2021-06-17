using System.Collections.Generic;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;

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

        public static ExampleModel ToDomain(this List<string> row)
        {
            return new ExampleModel
            {
                Name = row[0],
                Number = int.Parse(row[1]),
                IsHighlighted = bool.Parse(row[2])
            };
        }
    }
}
