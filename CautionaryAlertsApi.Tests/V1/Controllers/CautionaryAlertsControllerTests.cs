using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Controllers;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;
using CautionaryAlertsApi.V1.UseCase;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private Mock<IPropertyAlertsNewUseCase> _mockGetPropertyAlertsNewUseCase;
        private Mock<IGetCautionaryAlertsByPersonId> _mockGetCautionaryAlertsByPersonIdUseCase;
        private Mock<IPostNewCautionaryAlertUseCase> _mockPostNewCautionaryAlertUseCase;
        private Mock<ITokenFactory> _mockTokenFactory;
        private Mock<IHttpContextWrapper> _mockContextWrapper;
        private Mock<HttpRequest> _mockHttpRequest;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockGetAlertsForPersonUseCase = new Mock<IGetAlertsForPeople>();
            _mockGetAlertsForPropertyUseCase = new Mock<IGetCautionaryAlertsForProperty>();
            _mockGetPropertyAlertsNewUseCase = new Mock<IPropertyAlertsNewUseCase>();
            _mockGetCautionaryAlertsByPersonIdUseCase = new Mock<IGetCautionaryAlertsByPersonId>();
            _mockPostNewCautionaryAlertUseCase = new Mock<IPostNewCautionaryAlertUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpRequest = new Mock<HttpRequest>();

            new LogCallAspectFixture().RunBeforeTests();

            _classUnderTest = new CautionaryAlertsApiController(
                _mockGetAlertsForPersonUseCase.Object,
                _mockGetAlertsForPropertyUseCase.Object,
                _mockGetPropertyAlertsNewUseCase.Object,
                _mockGetCautionaryAlertsByPersonIdUseCase.Object,
                _mockPostNewCautionaryAlertUseCase.Object,
                _mockTokenFactory.Object,
                _mockContextWrapper.Object);

            _mockContextWrapper.Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>())).Returns(new HeaderDictionary());

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

        [Test]
        public async Task GetPropertyAlertsWhenCalledReturnsAlerts()
        {
            // Arrange
            var propertyReference = "0012345678";

            var usecaseResponse = _fixture.Create<CautionaryAlertsPropertyResponse>();

            _mockGetPropertyAlertsNewUseCase
                .Setup(x => x.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(usecaseResponse);

            // Act
            var response = await _classUnderTest.GetPropertyAlertsNew(propertyReference).ConfigureAwait(false) as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(usecaseResponse);
        }

        [Test]
        public async Task GetAlertsByPersonIdReturnsAlertsFromUseCase()
        {
            // Arrange
            var personId = Guid.NewGuid();

            var usecaseResponse = _fixture.Create<CautionaryAlertsMMHPersonResponse>();

            _mockGetCautionaryAlertsByPersonIdUseCase
                .Setup(x => x.ExecuteAsync(personId))
                .ReturnsAsync(usecaseResponse);

            // Act
            var response = await _classUnderTest.GetAlertsByPersonId(personId).ConfigureAwait(false) as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(usecaseResponse);
            _mockGetCautionaryAlertsByPersonIdUseCase.Verify(x => x.ExecuteAsync(personId), Times.Once);
        }

        [Test]
        public async Task CreateNewCautionaryAlertReturnsOkIfSuccessful()
        {
            // Arrange
            var createAlertResponse = _fixture.Create<CautionaryAlertListItem>();
            var createAlertRequest = _fixture.Create<CreateCautionaryAlert>();
            _mockPostNewCautionaryAlertUseCase
                .Setup(x => x.ExecuteAsync(createAlertRequest, It.IsAny<Token>()))
                .ReturnsAsync(createAlertResponse);

            // Act
            var response = await _classUnderTest.CreateNewCautionaryAlert(createAlertRequest).ConfigureAwait(false) as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            response.Value.Should().BeEquivalentTo(createAlertResponse);
            _mockPostNewCautionaryAlertUseCase.Verify(x => x.ExecuteAsync(createAlertRequest, It.IsAny<Token>()), Times.Once);
        }
    }
}
