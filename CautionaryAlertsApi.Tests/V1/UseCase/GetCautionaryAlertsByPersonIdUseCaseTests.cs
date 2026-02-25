using AutoFixture;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class GetCautionaryAlertsByPersonIdUseCaseTests
    {
        private readonly GetCautionaryAlertsByPersonIdUseCase _classUnderTest;
        private readonly Mock<IUhGateway> _mockGateway;

        private readonly Fixture _fixture = new Fixture();

        public GetCautionaryAlertsByPersonIdUseCaseTests()
        {
            _mockGateway = new Mock<IUhGateway>();
            _classUnderTest = new GetCautionaryAlertsByPersonIdUseCase(_mockGateway.Object);
        }

        [Test]
        public async Task ReturnsEmptyAlertsListWhenNoResults()
        {
            // Arrange
            var personId = Guid.NewGuid();

            _mockGateway
                .Setup(x => x.GetCautionaryAlertsByMMHPersonId(personId))
                .ReturnsAsync(new List<CautionaryAlertListItem>());

            // Act
            var result = await _classUnderTest.ExecuteAsync(personId).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.PersonId.Should().Be(personId);
            result.Alerts.Should().BeEmpty();
            _mockGateway.Verify(x => x.GetCautionaryAlertsByMMHPersonId(personId), Times.Once);
        }

        [Test]
        public async Task ReturnsAlertsFromGateway()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var mockAlerts = _fixture.Build<CautionaryAlertListItem>()
                                     .With(x => x.PersonId, personId.ToString())
                                     .With(x => x.AlertId, Guid.NewGuid().ToString())
                                     .CreateMany();

            _mockGateway
                .Setup(x => x.GetCautionaryAlertsByMMHPersonId(personId))
                .ReturnsAsync(mockAlerts);

            // Act
            var result = await _classUnderTest.ExecuteAsync(personId).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.PersonId.Should().Be(personId);
            result.Alerts.Should().HaveSameCount(mockAlerts);
            _mockGateway.Verify(x => x.GetCautionaryAlertsByMMHPersonId(personId), Times.Once);
        }
    }
}
