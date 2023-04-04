using AutoFixture;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions;
using Hackney.Shared.CautionaryAlerts.Domain;
using System;
using CautionaryAlertsApi.V1.Boundary.Request;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class EndCautionaryAlertUseCaseTests
    {
        private readonly EndCautionaryAlertUseCase _classUnderTest;
        private readonly Mock<IUhGateway> _mockGateway;
        private readonly Mock<ISnsGateway> _cautionaryAlertSnsGateway;
        private readonly CautionaryAlertsSnsFactory _cautionaryAlertsSnsFactory;

        private readonly Fixture _fixture = new Fixture();

        public EndCautionaryAlertUseCaseTests()
        {
            _mockGateway = new Mock<IUhGateway>();
            _cautionaryAlertSnsGateway = new Mock<ISnsGateway>();
            _cautionaryAlertsSnsFactory = new CautionaryAlertsSnsFactory();
            _classUnderTest = new EndCautionaryAlertUseCase(_mockGateway.Object, _cautionaryAlertSnsGateway.Object, _cautionaryAlertsSnsFactory);
        }

        [Test]
        public async Task ReturnsTheSameEntityThatIsGivenByTheGateway()
        {
            //Arrange 
            var token = new Token();
            var alertId = Guid.NewGuid();

            var mockExistingAlert = _fixture
                .Build<PropertyAlertDomain>()
                .With(a => a.AlertId, alertId.ToString())
                .Create();

            var endAlertData = _fixture.Create<AlertQueryObject>();
            var endAlertRequest = _fixture.Build<EndCautionaryAlertRequest>()
                                          .With(x => x.EndDate, DateTime.UtcNow.AddYears(-1))
                                          .Create();

            _mockGateway
                .Setup(x => x.EndCautionaryAlert(It.IsAny<AlertQueryObject>(), It.IsAny<EndCautionaryAlertRequest>()))
                .ReturnsAsync(mockExistingAlert);

            // Act
            var result = await _classUnderTest.ExecuteAsync(endAlertData, endAlertRequest, token).ConfigureAwait(false);

            // Assert
            _mockGateway.Verify(
                x => x.EndCautionaryAlert(It.IsAny<AlertQueryObject>(), It.IsAny<EndCautionaryAlertRequest>()),
                Times.Once
            );

            result.Should().NotBeNull();

            // Object references should match
            result.Should().Be(mockExistingAlert);
        }
    }
}
