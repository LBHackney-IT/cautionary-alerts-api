using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class GetAlertsForPersonTests
    {
        private Mock<IUhGateway> _mockGateway;
        private GetAlertsForPerson _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IUhGateway>();
            _classUnderTest = new GetAlertsForPerson(_mockGateway.Object);
        }

        [Test]
        public void ForTheGivenTagRefAndPersonNumberReturnsTheResponseFromTheGateway()
        {
            var tagRef = _fixture.Create<string>();
            var personNo = _fixture.Create<string>();
            var gatewayResponse = _fixture.CreateMany<CautionaryAlertPerson>().ToList();
            _mockGateway.Setup(x => x.GetCautionaryAlertsForAPerson(tagRef, personNo))
                .Returns(gatewayResponse);

            _classUnderTest.Execute(tagRef, personNo).Contacts.Should().BeEquivalentTo(gatewayResponse.ToResponse());
        }

        [Test]
        public void IfTheGatewayReturnsEmptyListThrowPersonNotFound()
        {
            _mockGateway.Setup(x =>
                    x.GetCautionaryAlertsForAPerson("tagRef", "personNo"))
                .Returns(new List<CautionaryAlertPerson>());

            Func<ListPersonsCautionaryAlerts> testDelegate = () => _classUnderTest.Execute("tagRef", "personNo");
            testDelegate.Should().Throw<PersonNotFoundException>();
        }
    }
}