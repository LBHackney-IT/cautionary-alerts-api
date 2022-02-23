using AutoFixture;
using CautionaryAlertsApi.V1.Infrastructure;
using CautionaryAlertsApi.V1.Factories;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void CanMapAPersonAlertToCautionaryAlertWithDescription()
        {
            // Arrange
            var entity = _fixture.Create<PersonAlert>();
            var description = _fixture.Create<string>();

            // Act
            var response = entity.ToDomain(description);

            // Assert
            response.Description.Should().Be(description);
            response.AlertCode.Should().Be(entity.AlertCode);
            response.DateModified.Should().Be(entity.DateModified);
            response.EndDate.Should().Be(entity.EndDate);
            response.StartDate.Should().Be(entity.StartDate);
            response.ModifiedBy.Should().Be(entity.ModifiedBy);
        }

        [Test]
        public void CanMapRowsToCautionaryAlertListItem()
        {
            // Arrange
            var numColumns = 16;
            var row = _fixture.CreateMany<string>(numColumns).ToList();

            // Act
            var result = EntityFactory.ToModel(row);

            // Assert
            result.Name.Should().Be(row[0]);
            result.DoorNumber.Should().Be(row[1]);
            result.Address.Should().Be(row[2]);
            result.Neighbourhood.Should().Be(row[3]);
            result.DateOfIncident.Should().Be(row[4]);
            result.NumberOfDaysOutstanding.Should().Be(row[5]);
            result.Code.Should().Be(row[6]);
            result.LetterSent.Should().Be(row[7]);
            result.OnCivica.Should().Be(row[8]);
            result.Outcome.Should().Be(row[9]);
            result.CautionOnSystem.Should().Be(row[10]);
            result.ActionOnAssure.Should().Be(row[11]);
            result.Lookup.Should().Be(row[12]);
            result.PropertyReference.Should().Be(row[13]);
            result.TenancyDates.Should().Be(row[14]);
            result.IncidentBeforeCurrentTenancyDate.Should().Be(row[15]);
        }

        [Test]
        public void DoesntThrowIndexOutOfRangeExceptionWhenMissingColumns()
        {
            // Arrange
            var numColumns = 16;
            var realNumColumns = numColumns - 1;

            var row = _fixture.CreateMany<string>(realNumColumns).ToList();

            // Act
            Func<CautionaryAlertListItem> task = () => EntityFactory.ToModel(row);

            // Assert
            task.Should().NotThrow<IndexOutOfRangeException>();
        }
    }
}
