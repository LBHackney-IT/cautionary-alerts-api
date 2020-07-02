using System.Linq;
using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class UhContextTest : DatabaseTests
    {
        [Test]
        public void CanGetADatabaseEntity()
        {
            var fixture = new Fixture();
            var databaseEntity = fixture.Create<PersonAlert>();
            UhContext.PeopleAlerts.Add(databaseEntity);
            UhContext.SaveChanges();

            var result = UhContext.PeopleAlerts.ToList().FirstOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
