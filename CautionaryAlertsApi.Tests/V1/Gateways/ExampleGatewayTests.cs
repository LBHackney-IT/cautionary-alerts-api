using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Gateways
{
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-base-api/wiki/Running-the-test-suite.
    [TestFixture]
    public class ExampleGatewayTests : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private ExampleGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new ExampleGateway(UhContext);
        }

        [Test]
        public void GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var response = _classUnderTest.GetEntityById(123);

            response.Should().BeNull();
        }

        [Test]
        public void GetEntityByIdReturnsTheEntityIfItExists()
        {
            var entity = _fixture.Create<CautionaryAlert>();
            var databaseEntity = DatabaseEntityHelper.CreatePeopleAlert();

            UhContext.PeopleAlerts.Add(databaseEntity);
            UhContext.SaveChanges();

            var response = _classUnderTest.GetEntityById(databaseEntity.Id);

            databaseEntity.Id.Should().Be(response.Id);
        }

        //TODO: Add tests here for the get all method.
    }
}
