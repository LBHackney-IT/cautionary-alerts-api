using AutoFixture;
using CautionaryAlertsApi.V1.Infrastructure;
using CautionaryAlertsApi.V1.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        [Test]
        public void CanMapAPersonAlertToCautionaryAlertWithDescription()
        {
            var fixture = new Fixture();
            var entity = fixture.Create<PersonAlert>();
            var description = fixture.Create<string>();
            var response = entity.ToDomain(description);

            response.Description.Should().Be(description);
            response.AlertCode.Should().Be(entity.AlertCode);
            response.DateModified.Should().Be(entity.DateModified);
            response.EndDate.Should().Be(entity.EndDate);
            response.StartDate.Should().Be(entity.StartDate);
            response.ModifiedBy.Should().Be(entity.ModifiedBy);
        }
    }
}
