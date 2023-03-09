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
using Hackney.Shared.CautionaryAlerts.Boundary.Request;

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
            var query = _fixture.Create<AlertQueryObject>();
            var mockAlert = _fixture.Build<CautionaryAlert>()
                                     .With(x => x.PersonId, query.PersonId)
                                     .With(x => x.AlertId, query.AlertId)
                                     .Create();

            _mockGateway
                .Setup(x => x.GetCautionaryAlertByAlertId(query))
                .Returns(mockAlert);

            // Act
            var result = _classUnderTest.ExecuteAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.PersonId.Should().Be(query.PersonId);
            result.AlertId.Should().Be(query.AlertId);
            _mockGateway.Verify(x => x.GetCautionaryAlertByAlertId(query), Times.Once);
        }
    }
}
