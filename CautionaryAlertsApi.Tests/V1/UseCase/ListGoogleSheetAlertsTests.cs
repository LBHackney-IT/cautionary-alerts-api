using System.Collections.Generic;
using AutoFixture;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class ListGoogleSheetAlertsTests
    {
        private Mock<IGoogleSheetGateway> _gateway;
        private ListGoogleSheetAlerts _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _gateway = new Mock<IGoogleSheetGateway>();
            _classUnderTest = new ListGoogleSheetAlerts(_gateway.Object);
        }

        [Test]
        public void WillCallGatewayAndRetrieveCautionaryAlertListItems()
        {
            // Arrange
            var expectedResponse = _fixture.Create<List<CautionaryAlertListItem>>();
            _gateway
                .Setup(g => g.ListPropertyAlerts())
                .Returns(expectedResponse);

            // Act
            var result = _classUnderTest.Execute();

            // Assert
            _gateway.Verify(g => g.ListPropertyAlerts(), Times.Once);
            result.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
