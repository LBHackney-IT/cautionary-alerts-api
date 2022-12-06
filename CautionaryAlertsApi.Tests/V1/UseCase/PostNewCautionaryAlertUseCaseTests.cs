using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.Infrastructure;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

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
            var defaultString = string.Join("", _fixture.CreateMany<char>(CreateCautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var cautionaryAlert = CreateCautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture);
            var token = new Token();

            //var cautionaryAlertDb = cautionaryAlert.ToDatabase();
            //var cautionaryToDomain = cautionaryAlertDb.ToDomain();


            var cautionaryAlertDb = _fixture.Build<PropertyAlertNew>()
                                            .With(x => x.MMHID, cautionaryAlert.PersonDetails.Id.ToString())
                                            .With(x => x.Address, cautionaryAlert.AssetDetails.FullAddress)
                                            .With(x => x.AssureReference, cautionaryAlert.AssureReference)
                                            .With(x => x.CautionOnSystem, cautionaryAlert.Alert.Description)
                                            .With(x => x.Code, cautionaryAlert.Alert.Code)
                                            .With(x => x.DateOfIncident, cautionaryAlert.IncidentDate.ToString("d", CultureInfo.InvariantCulture))
                                            .With(x => x.UPRN, cautionaryAlert.AssetDetails.UPRN)
                                            .With(x => x.PropertyReference, cautionaryAlert.AssetDetails.PropertyReference)
                                            .With(x => x.PersonName, cautionaryAlert.PersonDetails.Name)
                                            .With(x => x.Reason, cautionaryAlert.IncidentDescription)
                                            .Create();

            _mockGateway
                .Setup(x => x.PostNewCautionaryAlert(cautionaryAlert))
                .ReturnsAsync(cautionaryAlertDb.ToDomain());

            // Act
            var result = await _classUnderTest.ExecuteAsync(cautionaryAlert, token).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<CautionaryAlertListItem>();

        }
    }
}
