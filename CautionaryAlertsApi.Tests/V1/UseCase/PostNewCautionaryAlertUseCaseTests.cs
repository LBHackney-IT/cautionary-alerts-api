using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using CautionaryAlertsApi.V1.Gateways;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class PostNewCautionaryAlertUseCaseTests
    {
        private readonly PostNewCautionaryAlertUseCase _classUnderTest;
        private readonly Mock<IUhGateway> _mockGateway;

        private readonly Fixture _fixture = new Fixture();

        public PostNewCautionaryAlertUseCaseTests()
        {
            _mockGateway = new Mock<IUhGateway>();
            _classUnderTest = new PostNewCautionaryAlertUseCase(_mockGateway.Object);
        }

        [Test]
        public async Task ReturnsCautionaryAlertListItemIfSuccessful()
        {
            var defaultString = string.Join("", _fixture.CreateMany<char>(CreateCautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var cautionaryAlert = CreateCautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture);

            _mockGateway
                .Setup(x => x.PostNewCautionaryAlert(It.IsAny<CreateCautionaryAlert>()))
                .ReturnsAsync(new CautionaryAlertListItem());

            // Act
            var result = await _classUnderTest.ExecuteAsync(cautionaryAlert).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<CautionaryAlertListItem>();
        }
    }
}
