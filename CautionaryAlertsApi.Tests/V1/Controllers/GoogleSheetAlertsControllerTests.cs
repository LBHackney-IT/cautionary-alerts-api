using CautionaryAlertsApi.V1.Controllers;
using CautionaryAlertsApi.V1.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using CautionaryAlertsApi.V1.UseCase;
using AutoFixture;
using CautionaryAlertsApi.V1.Gateways;

namespace CautionaryAlertsApi.Tests.V1.Controllers
{
    public class GoogleSheetAlertsControllerTests
    {
        private GoogleSheetAlertsController _classUnderTest;
        private Mock<IGetGoogleSheetAlertsForProperty> _getAlertsMock;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _getAlertsMock = new Mock<IGetGoogleSheetAlertsForProperty>();
            _classUnderTest = new GoogleSheetAlertsController(_getAlertsMock.Object);
        }

        [Test]
        public void CanListAlertsForProperty()
        {
            // Arrange
            const string propertyReference = "0012345678";
            _fixture.Customize<CautionaryAlertListItem>(c => c.With(li => li.PropertyReference, propertyReference));

            var expectedUseCaseResponse = _fixture.Create<List<CautionaryAlertListItem>>();
            _getAlertsMock.Setup(x => x.Execute(propertyReference)).Returns(expectedUseCaseResponse);

            var response = _classUnderTest.GetAlertsByProperty(propertyReference) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(expectedUseCaseResponse);
        }

        [Test]
        public void ReturnsNotFoundWhenPropertyAlertNotFoundException()
        {
            const string propertyReference = "0012345678";
            _getAlertsMock.Setup(x => x.Execute(propertyReference)).Throws<PropertyAlertNotFoundException>();

            var response = _classUnderTest.GetAlertsByProperty(propertyReference) as ObjectResult;

            response.Should().NotBeNull();
            response.Value.Should().Be($"Cautionary alert(s) for property reference {propertyReference} not found");
            response.StatusCode.Should().Be(404);
        }
    }
}
