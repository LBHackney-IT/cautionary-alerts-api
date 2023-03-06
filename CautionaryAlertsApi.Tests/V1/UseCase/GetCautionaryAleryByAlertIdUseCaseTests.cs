using AutoFixture;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FluentAssertions;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class GetCautionaryAleryByAlertIdUseCaseTests
    {
        private readonly GetCautionaryAlertByAlertIdUseCase _classUnderTest;
        private readonly Mock<IUhGateway> _mockGateway;

        private readonly Fixture _fixture = new Fixture();

        public GetCautionaryAleryByAlertIdUseCaseTests()
        {
            _mockGateway = new Mock<IUhGateway>();
            _classUnderTest = new GetCautionaryAlertByAlertIdUseCase(_mockGateway.Object);
        }

        [Test]
        public void ReturnsAlertByAlertIdFromGateway()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var alertId = Guid.NewGuid();
            var mockAlerts = _fixture.Build<CautionaryAlert>()
                                     .With(x => x.PersonId, personId)
                                     .With(x => x.AlertId, alertId)
                                     .Create();

            _mockGateway
                .Setup(x => x.GetCautionaryAlertByAlertId(personId, alertId))
                .Returns(mockAlerts);

            // Act
            var result = _classUnderTest.ExecuteAsync(personId, alertId);

            // Assert
            result.Should().NotBeNull();
            result.PersonId.Should().Be(personId);
            result.AlertId.Should().Be(alertId);
            _mockGateway.Verify(x => x.GetCautionaryAlertByAlertId(personId, alertId), Times.Once);
        }
    }
}
