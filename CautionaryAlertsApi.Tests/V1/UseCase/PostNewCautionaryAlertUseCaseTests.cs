using System;
using AutoFixture;
using CautionaryAlertsApi.V1.Gateways;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using CautionaryAlertsApi.V1.Factories;
using Hackney.Shared.CautionaryAlerts.Factories;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class PostNewCautionaryAlertUseCaseTests
    {
        private readonly PostNewCautionaryAlertUseCase _classUnderTest;
        private readonly Mock<IUhGateway> _mockGateway;
        private readonly Mock<ISnsGateway> _cautionaryAlertSnsGateway;
        private readonly CautionaryAlertsSnsFactory _cautionaryAlertsSnsFactory;

        private readonly Fixture _fixture = new Fixture();

        public PostNewCautionaryAlertUseCaseTests()
        {
            _mockGateway = new Mock<IUhGateway>();
            _cautionaryAlertSnsGateway = new Mock<ISnsGateway>();
            _cautionaryAlertsSnsFactory = new CautionaryAlertsSnsFactory();
            _classUnderTest = new PostNewCautionaryAlertUseCase(_mockGateway.Object, _cautionaryAlertSnsGateway.Object, _cautionaryAlertsSnsFactory);
        }

        [Test]
        public async Task ReturnsCautionaryAlertListItemIfSuccessful()
        {
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));
            var cautionaryAlert = CreateCautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture, addressString);
            var token = new Token();
            var alertId = _fixture.Create<Guid>().ToString();
            var cautionaryAlertDb = cautionaryAlert.ToDatabase(isActive: true, alertId);

            var cautionaryToDomain = cautionaryAlertDb.ToPropertyAlertDomain();


            _mockGateway
                .Setup(x => x.PostNewCautionaryAlert(cautionaryAlert))
                .ReturnsAsync(cautionaryToDomain);

            // Act
            var result = await _classUnderTest.ExecuteAsync(cautionaryAlert, token).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<CautionaryAlertListItem>();

        }
    }
}
