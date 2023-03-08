using AutoFixture;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using FluentAssertions;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Domain;

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
        public async Task ReturnsCautionaryAlertListItemIfSuccessful()
        {
            //Arrange
            var endCautionaryAlert = _fixture.Create<EndCautionaryAlert>();

            var token = new Token();

            var cautionaryAlertDb = endCautionaryAlert.ToDatabase();
            var cautionaryToDomain = cautionaryAlertDb.ToPropertyAlertDomain();


            _mockGateway
                .Setup(x => x.EndCautionaryAlert(endCautionaryAlert))
                .ReturnsAsync(cautionaryToDomain);

            // Act
            var result = await _classUnderTest.ExecuteAsync(endCautionaryAlert, token).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PropertyAlertDomain>();

        }
    }
}
