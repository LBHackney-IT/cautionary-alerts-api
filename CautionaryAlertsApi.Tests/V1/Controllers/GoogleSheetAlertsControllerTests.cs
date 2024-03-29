using CautionaryAlertsApi.V1.Controllers;
using Hackney.Shared.CautionaryAlerts.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using CautionaryAlertsApi.V1.UseCase;
using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using CautionaryAlertsApi.V1.UseCase.Interfaces;

namespace CautionaryAlertsApi.Tests.V1.Controllers
{
    public class GoogleSheetAlertsControllerTests
    {
        private GoogleSheetAlertsController _classUnderTest;
        private Mock<IGetGoogleSheetAlertsForProperty> _getPropertyAlertsMock;
        private Mock<IGetGoogleSheetAlertsForPerson> _getPersonAlertsMock;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _getPropertyAlertsMock = new Mock<IGetGoogleSheetAlertsForProperty>();
            _getPersonAlertsMock = new Mock<IGetGoogleSheetAlertsForPerson>();
            _classUnderTest = new GoogleSheetAlertsController(_getPropertyAlertsMock.Object, _getPersonAlertsMock.Object);
        }

        [Test]
        public void CanListAlertsForProperty()
        {
            // Arrange
            const string propertyReference = "0012345678";
            _fixture.Customize<CautionaryAlertListItem>(c => c.With(li => li.PropertyReference, propertyReference));

            var expectedUseCaseResponse = _fixture.Create<CautionaryAlertsPropertyResponse>();
            _getPropertyAlertsMock.Setup(x => x.Execute(propertyReference)).Returns(expectedUseCaseResponse);

            var response = _classUnderTest.GetAlertsByProperty(propertyReference) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(expectedUseCaseResponse);
        }

        [Test]
        public void ReturnsNotFoundWhenPropertyAlertNotFoundException()
        {
            const string propertyReference = "0012345678";
            _getPropertyAlertsMock.Setup(x => x.Execute(propertyReference)).Throws<PropertyAlertNotFoundException>();

            var response = _classUnderTest.GetAlertsByProperty(propertyReference) as ObjectResult;

            response.Should().NotBeNull();
            response.Value.Should().Be($"Cautionary alert(s) for property reference {propertyReference} not found");
            response.StatusCode.Should().Be(404);
        }
    }
}
