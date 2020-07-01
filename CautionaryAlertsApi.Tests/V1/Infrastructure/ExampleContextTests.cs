using System.Linq;
using CautionaryAlertsApi.Tests.V1.Helper;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class DatabaseContextTest : DatabaseTests
    {
        [Test]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = DatabaseEntityHelper.CreateDatabaseEntity();

            DatabaseContext.Add(databaseEntity);
            DatabaseContext.SaveChanges();

            var result = DatabaseContext.PeopleAlerts.ToList().FirstOrDefault();

            Assert.AreEqual(result, databaseEntity);
        }
    }
}