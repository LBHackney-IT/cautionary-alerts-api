using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetGoogleSheetAlertsForPropertyTests
    {
        private Mock<IGoogleSheetGateway> _gateway;
        private GetGoogleSheetAlertsForProperty _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _gateway = new Mock<IGoogleSheetGateway>();
            _classUnderTest = new GetGoogleSheetAlertsForProperty(_gateway.Object);
        }

        [Test]
        public void WillCallGatewayAndRetrieveCautionaryAlertListItem()
        {
            // Arrange
            var expectedResponse = new List<CautionaryAlertListItem> { _fixture.Create<CautionaryAlertListItem>() };
            var propertyReference = expectedResponse.First().PropertyReference;
            _gateway
                .Setup(g => g.GetPropertyAlerts(propertyReference))
                .Returns(expectedResponse);

            // Act
            var result = _classUnderTest.Execute(propertyReference);

            // Assert
            _gateway.Verify(g => g.GetPropertyAlerts(propertyReference), Times.Once);
            result.Alerts.Should().BeEquivalentTo(expectedResponse.Select(l => l.ToResponse()));
        }
    }
}
