using AutoFixture;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;

namespace CautionaryAlertsApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static DatabaseEntity CreateDatabaseEntity()
        {
            var entity = new Fixture().Create<CautionaryAlert>();

            return CreateDatabaseEntityFrom(entity);
        }

        public static DatabaseEntity CreateDatabaseEntityFrom(CautionaryAlert cautionaryAlert)
        {
            return new DatabaseEntity
            {
                Id = cautionaryAlert.Id,
                CreatedAt = cautionaryAlert.CreatedAt,
            };
        }
    }
}
