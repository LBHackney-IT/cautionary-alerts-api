using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;

namespace CautionaryAlertsApi.V1.Factories
{
    public static class EntityFactory
    {
        public static void ToDomain(this PersonAlert personAlert)
        {
            //TODO: Map the rest of the fields in the domain object.
            // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings

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
    }
}
