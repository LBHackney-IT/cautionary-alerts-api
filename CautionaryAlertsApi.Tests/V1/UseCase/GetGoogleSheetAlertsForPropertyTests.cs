using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using System;

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
            var cautionaryAlertList = new List<CautionaryAlertListItem>();

            var cautionaryAlert = _fixture.Build<CautionaryAlertListItem>()
                                          .With(x => x.PersonId, Guid.NewGuid().ToString())
                                          .With(x => x.AlertId, Guid.NewGuid().ToString())
                                          .Create();

            cautionaryAlertList.Add(cautionaryAlert);

            var propertyReference = cautionaryAlertList.First().PropertyReference;

            _gateway
                .Setup(g => g.GetPropertyAlerts(propertyReference))
                .Returns(cautionaryAlertList);

            // Act
            var result = _classUnderTest.Execute(propertyReference);

            // Assert
            _gateway.Verify(g => g.GetPropertyAlerts(propertyReference), Times.Once);
            result.Alerts.Should().BeEquivalentTo(cautionaryAlertList.Select(l => l.ToResponse()));
        }
    }
}
