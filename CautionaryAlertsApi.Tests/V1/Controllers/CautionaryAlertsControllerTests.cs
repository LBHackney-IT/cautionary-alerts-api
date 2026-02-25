using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using CautionaryAlertsApi.V1.Controllers;
using Hackney.Shared.CautionaryAlerts.Domain;
using CautionaryAlertsApi.V1.UseCase;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using Hackney.Core.Http;
using Hackney.Core.JWT;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Boundary.Request;

namespace CautionaryAlertsApi.Tests.V1.Controllers
{
    public class CautionaryAlertsControllerTests
    {
        private CautionaryAlertsApiController _classUnderTest;
        private Mock<IGetAlertsForPeople> _mockGetAlertsForPersonUseCase;
        private Mock<IGetCautionaryAlertsForProperty> _mockGetAlertsForPropertyUseCase;
        private Mock<IPropertyAlertsNewUseCase> _mockGetPropertyAlertsNewUseCase;
        private Mock<IGetCautionaryAlertsByPersonId> _mockGetCautionaryAlertsByPersonIdUseCase;
        private Mock<IGetCautionaryAlertByAlertIdUseCase> _mockGetCautionaryAlertByAlertIdUseCase;
        private Mock<IPostNewCautionaryAlertUseCase> _mockPostNewCautionaryAlertUseCase;
        private Mock<IEndCautionaryAlertUseCase> _mockEndCautionaryAlertUseCase;
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
            _mockGetCautionaryAlertByAlertIdUseCase = new Mock<IGetCautionaryAlertByAlertIdUseCase>();
            _mockPostNewCautionaryAlertUseCase = new Mock<IPostNewCautionaryAlertUseCase>();
            _mockEndCautionaryAlertUseCase = new Mock<IEndCautionaryAlertUseCase>();
            _mockTokenFactory = new Mock<ITokenFactory>();
            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpRequest = new Mock<HttpRequest>();

            new LogCallAspectFixture().RunBeforeTests();

            _classUnderTest = new CautionaryAlertsApiController(
                _mockGetAlertsForPersonUseCase.Object,
                _mockGetAlertsForPropertyUseCase.Object,
                _mockGetPropertyAlertsNewUseCase.Object,
                _mockGetCautionaryAlertsByPersonIdUseCase.Object,
                _mockGetCautionaryAlertByAlertIdUseCase.Object,
                _mockPostNewCautionaryAlertUseCase.Object,
                _mockEndCautionaryAlertUseCase.Object,
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
        public void GetAlertByAlertIdReturnsAlertsFromUseCase()
        {
            // Arrange
            var query = _fixture.Create<AlertQueryObject>();

            var usecaseResponse = _fixture.Create<PropertyAlertDomain>();


            _mockGetCautionaryAlertByAlertIdUseCase
                .Setup(x => x.ExecuteAsync(query))
                .Returns(usecaseResponse);

            // Act
            var response = _classUnderTest.GetAlertByAlertId(query) as OkObjectResult;

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(usecaseResponse);
            _mockGetCautionaryAlertByAlertIdUseCase.Verify(x => x.ExecuteAsync(query), Times.Once);
        }

        [Test]
        public void GetAlertByAlertIdReturns404()
        {
            // Arrange
            var query = _fixture.Create<AlertQueryObject>();


            _mockGetCautionaryAlertByAlertIdUseCase
                .Setup(x => x.ExecuteAsync(query))
                .Returns((PropertyAlertDomain) null);

            // Act
            var response = _classUnderTest.GetAlertByAlertId(query) as NotFoundObjectResult;

            // Assert
            response.StatusCode.Should().Be(404);
            response.Value.Should().Be(query.AlertId);
            _mockGetCautionaryAlertByAlertIdUseCase.Verify(x => x.ExecuteAsync(query), Times.Once);
        }

        [Test]
        public void GetAlertByAlertIdThrowsException()
        {
            // Arrange
            var query = _fixture.Create<AlertQueryObject>();

            var exception = new ApplicationException("Test exception");

            _mockGetCautionaryAlertByAlertIdUseCase
                .Setup(x => x.ExecuteAsync(query))
                .Throws(exception);

            // Act
            Func<Task<IActionResult>> func = () => (Task<IActionResult>) _classUnderTest.GetAlertByAlertId(query);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);

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

        [Test]
        public async Task EndCautionaryAlertReturnsNoContentIfSuccessful()
        {
            // Arrange
            var alertQueryObj = _fixture.Create<AlertQueryObject>();
            var endAlertRequest = _fixture.Build<EndCautionaryAlertRequest>()
                                          .With(x => x.EndDate, DateTime.UtcNow.AddYears(-1))
                                          .Create();
            var createAlertDomain = _fixture.Create<PropertyAlertDomain>();

            _mockEndCautionaryAlertUseCase
                .Setup(x => x.ExecuteAsync(It.IsAny<AlertQueryObject>(), It.IsAny<EndCautionaryAlertRequest>(), It.IsAny<Token>()))
                .ReturnsAsync(createAlertDomain);

            // Act
            var response = await _classUnderTest.EndCautionaryAlert(alertQueryObj, endAlertRequest).ConfigureAwait(false) as NoContentResult;

            // Assert
            _mockEndCautionaryAlertUseCase.Verify(
                x => x.ExecuteAsync(It.Is<AlertQueryObject>(e => e.AlertId == alertQueryObj.AlertId), It.Is<EndCautionaryAlertRequest>(endAlertRequest => endAlertRequest.EndDate == endAlertRequest.EndDate), It.IsAny<Token>()),
                Times.Once
            );

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
    }
}
