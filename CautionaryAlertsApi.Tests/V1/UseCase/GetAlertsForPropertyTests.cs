using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class GetAlertsForPropertyTests
    {
        private Mock<IUhGateway> _mockGateway;
        private GetCautionaryAlertsForProperty _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IUhGateway>();
            _classUnderTest = new GetCautionaryAlertsForProperty(_mockGateway.Object);
        }

        [Test]
        public void VerifyThatGatewayIsCalled()
        {
            var propertyReference = "00101010101";
            var expectedGatewayResponse = _fixture.Create<CautionaryAlertsProperty>();
            _mockGateway.Setup(x => x.GetCautionaryAlertsForAProperty(It.IsAny<string>())).Returns(expectedGatewayResponse);
            _classUnderTest.Execute(propertyReference);

            _mockGateway.Verify(x => x.GetCautionaryAlertsForAProperty(propertyReference), Times.Once);
        }

        [Test]
        public void ForAGivenPropertyReferenceResponseIsReturnedFromGateway()
        {
            var expectedGatewayResponse = _fixture.Create<CautionaryAlertsProperty>();
            _mockGateway.Setup(x => x.GetCautionaryAlertsForAProperty(It.IsAny<string>())).Returns(expectedGatewayResponse);

            var response = _classUnderTest.Execute(It.IsAny<string>());

            response.Should().BeEquivalentTo(ResponseFactory.ToResponse(expectedGatewayResponse));
        }

        [Test]
        public void ShouldThrowExceptionIfGatewayReturnsNoResults()
        {
            _mockGateway.Setup(x => x.GetCautionaryAlertsForAProperty(It.IsAny<string>())).Returns(() => null);

            Func<CautionaryAlertsPropertyResponse> testDelegate = () => _classUnderTest.Execute(It.IsAny<string>());
            testDelegate.Should().Throw<PropertyAlertNotFoundException>();
        }
    }
}
