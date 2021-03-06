using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Controllers;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.Tests.V1.Controllers
{
    public class CautionaryAlertsControllerTests
    {
        private CautionaryAlertsApiController _classUnderTest;
        private Mock<IGetAlertsForPeople> _mockGetAlertsForPersonUseCase;
        private Mock<IGetCautionaryAlertsForProperty> _mockGetAlertsForPropertyUseCase;

        [SetUp]
        public void SetUp()
        {
            _mockGetAlertsForPersonUseCase = new Mock<IGetAlertsForPeople>();
            _mockGetAlertsForPropertyUseCase = new Mock<IGetCautionaryAlertsForProperty>();
            _classUnderTest = new CautionaryAlertsApiController(_mockGetAlertsForPersonUseCase.Object, _mockGetAlertsForPropertyUseCase.Object);
        }

        [Test]
        public void CanListAlertsForProperty()
        {
            var expectedUseCaseResponse = new CautionaryAlertsPropertyResponse()
            {
                AddressNumber = "123",
                PropertyReference = "0012345678",
                Alerts = new List<CautionaryAlertResponse>()
            };

            _mockGetAlertsForPropertyUseCase.Setup(x => x.Execute(It.IsAny<string>())).Returns(expectedUseCaseResponse);

            var response = _classUnderTest.ViewPropertyCautionaryAlerts(It.IsAny<string>()) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(expectedUseCaseResponse);
        }

        [Test]
        public void CatchesAndReturnsNotFoundWhenUseCaseThrowsPropertyAlertNotFoundException()
        {
            var propertyReference = "0012345678";
            _mockGetAlertsForPropertyUseCase.Setup(x => x.Execute(propertyReference)).Throws<PropertyAlertNotFoundException>();

            var response = _classUnderTest.ViewPropertyCautionaryAlerts(propertyReference) as NotFoundObjectResult;

            response.Should().NotBeNull();
            response.Value.Should().Be($"Property cautionary alert(s) for property reference {propertyReference} not found");
            response.StatusCode.Should().Be(404);
        }
    }
}
