using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class UhGatewayTests : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private UhGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UhGateway(UhContext);
        }

        [Test]
        public void GetCautionaryAlertsForAPersonReturnsAnEmptyListIfNoneExistAgainstThePerson()
        {
            var response = _classUnderTest.GetCautionaryAlertsForAPerson("0101/4", "2686");

            response.Should().BeEmpty();
        }
    }
}
