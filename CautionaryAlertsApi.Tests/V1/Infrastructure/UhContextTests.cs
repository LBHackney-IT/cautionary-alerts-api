using System.Linq;
using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
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
        [Test]
        public void CanGetADatabaseEntityOfTypePropertyAlert()
        {
            var fixture = new Fixture();
            var databaseEntity = fixture.Create<PropertyAlert>();
            UhContext.PropertyAlerts.Add(databaseEntity);
            UhContext.SaveChanges();

            var result = UhContext.PropertyAlerts.ToList().FirstOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
