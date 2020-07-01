using AutoFixture;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;

namespace CautionaryAlertsApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static PersonAlert CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<CautionaryAlert>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static PersonAlert CreateDatabaseEntityFrom(CautionaryAlert cautionaryAlert)
        {
            return new PersonAlert
            {
                Id = cautionaryAlert.Id,
                ContactNumber = cautionaryAlert.CreatedAt,
            };
        }
    }
}
