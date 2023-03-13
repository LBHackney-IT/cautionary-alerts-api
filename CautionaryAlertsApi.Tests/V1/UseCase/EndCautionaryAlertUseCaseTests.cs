using AutoFixture;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.CautionaryAlerts.Factories;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using FluentAssertions;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using System;

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
            var alertId = Guid.NewGuid();
            var personId = Guid.NewGuid();
            var mockExistingAlert = _fixture.Build<PropertyAlertDomain>()
                                     .With(x => x.MMHID, personId.ToString())
                                     .With(x => x.AlertId, alertId.ToString())
                                     .Create();




            _mockGateway
               .Setup(x => x.GetCautionaryAlertByAlertId(It.Is<AlertQueryObject>(x => x.AlertId == alertId && x.PersonId == personId)))
               .Returns(mockExistingAlert);

            mockExistingAlert.IsActive = false;

            var personDetail = _fixture.Build<PersonDetails>().With(x => x.Id, personId).Create();
            var endCautionaryAlert = _fixture.Build<EndCautionaryAlert>()
                                                .With(x => x.AlertId, alertId)
                                                .With(x => x.PersonDetails, personDetail)
                                                .Create();
            var token = new Token();
            _mockGateway
                .Setup(x => x.EndCautionaryAlert(It.Is<PropertyAlertNew>(x => x.AlertId == alertId.ToString() && x.MMHID == personId.ToString())))
                .ReturnsAsync(mockExistingAlert);

            // Act
             var result = await _classUnderTest.ExecuteAsync(endCautionaryAlert,
                                                             token).ConfigureAwait(false);

            // Assert
            _mockGateway.Verify(x => x.GetCautionaryAlertByAlertId(It.IsAny<AlertQueryObject>()), Times.Once);
            _mockGateway.Verify(x => x.EndCautionaryAlert(It.IsAny<PropertyAlertNew>()), Times.Once);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<PropertyAlertDomain>();

        }
    }
}
