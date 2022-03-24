using AutoFixture;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class GetPropertyAlertsNewUseCaseTests
    {
        private readonly GetPropertyAlertsNewUseCase _classUnderTest;
        private readonly Mock<IUhGateway> _mockGateway;

        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        public GetPropertyAlertsNewUseCaseTests()
        {
            _mockGateway = new Mock<IUhGateway>();

            _classUnderTest = new GetPropertyAlertsNewUseCase(_mockGateway.Object);
        }

        [Test]
        public async Task WhenNoResultsFoundReturnsEmptyResponse()
        {
            // Arrange
            var propertyReference = "00001234";

            _mockGateway
                .Setup(x => x.GetPropertyAlertsNew(It.IsAny<string>()))
                .ReturnsAsync(new List<CautionaryAlertListItem>());

            // Act
            var result = await _classUnderTest.ExecuteAsync(propertyReference).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.PropertyReference.Should().Be(propertyReference);
            result.Alerts.Should().BeEmpty();
        }

        [Test]
        public async Task WhenCalledReturnsAlerts()
        {
            // Arrange
            var propertyReference = "00001234";
            var numberOfResults = _random.Next(2, 5);

            var alerts = _fixture.CreateMany<CautionaryAlertListItem>(numberOfResults);

            _mockGateway
                .Setup(x => x.GetPropertyAlertsNew(It.IsAny<string>()))
                .ReturnsAsync(alerts);

            // Act
            var result = await _classUnderTest.ExecuteAsync(propertyReference).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.PropertyReference.Should().Be(propertyReference);
            result.Alerts.Should().HaveCount(numberOfResults);
        }
    }
}
