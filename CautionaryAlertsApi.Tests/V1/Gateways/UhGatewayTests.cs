using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CautionaryAlertsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class UhGatewayTests : DatabaseTests
    {
        private UhGateway _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UhGateway(UhContext);
            _fixture = new Fixture();
        }

        [Test]
        public void GetCautionaryAlertsForAPersonReturnsAnEmptyListIfNoneExistAgainstThePerson()
        {
            var response = _classUnderTest.GetCautionaryAlertsForAPerson("0101/4", "2686");

            response.Should().BeEmpty();
        }

        [Test]
        public void GetCautionaryAlertsForAPropertyReturnsEmtpyListIfNoMatchFoundForPropertyReference()
        {
            var response = _classUnderTest.GetCautionaryAlertsForAProperty("0010101010");

            response.Should().BeNull();
        }

        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAllAlertsAddedAgainstAPropertyAsAList()
        {
            var addressLink = AddAddressLinkToDb();
            //alert 1
            var alert = AddAlertToDatabaseForProperty(addressLink.AddressNumber);
            var descAlertOne = AddDescriptionToDatabase(alert.AlertCode);
            //alert2
            var secondAlert = AddAlertToDatabaseForProperty(addressLink.AddressNumber);
            var descAlertTwo = AddDescriptionToDatabase(secondAlert.AlertCode);

            var expectedResponse = new CautionaryAlertsProperty() { AddressNumber = addressLink.AddressNumber.ToString(), PropertyReference = addressLink.PropertyReference };
            expectedResponse.Alerts = new List<CautionaryAlert>() { alert.ToDomain(descAlertOne.Description), secondAlert.ToDomain(descAlertTwo.Description) };

            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.Should().BeEquivalentTo(expectedResponse);
            response.Alerts.Count.Should().Be(2);
        }

        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAlertsWithDescriptions()
        {
            var addressLink = AddAddressLinkToDb();
            var alert = AddAlertToDatabaseForProperty(addressLink.AddressNumber);
            var descAlertOne = AddDescriptionToDatabase(alert.AlertCode);

            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.Alerts.First().Description.Should().BeEquivalentTo(descAlertOne.Description);
        }

        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAddressNumberForProperty()
        {
            var addressLink = AddAddressLinkToDb();
            var alert = AddAlertToDatabaseForProperty(addressLink.AddressNumber);

            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.AddressNumber.Should().BeEquivalentTo(addressLink.AddressNumber.ToString());
        }


        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAlertsInformation()
        {
            var addressLink = AddAddressLinkToDb();
            var alert = AddAlertToDatabaseForProperty(addressLink.AddressNumber);
            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.Alerts.First().StartDate.Should().BeSameDateAs(alert.StartDate);
            response.Alerts.First().DateModified.Should().BeSameDateAs(alert.DateModified);
            response.Alerts.First().EndDate.Should().BeNull();
            response.Alerts.First().ModifiedBy.Should().BeEquivalentTo(alert.ModifiedBy);
            response.Alerts.First().AlertCode.Should().BeEquivalentTo(alert.AlertCode);
        }


        private AddressLink AddAddressLinkToDb(string propertyReference = null, int? id = null)
        {
            var addressLink = _fixture.Create<AddressLink>();
            addressLink.PropertyReference = propertyReference ?? addressLink.PropertyReference;
            addressLink.AddressNumber = id ?? addressLink.AddressNumber;
            addressLink.DateModified =addressLink.DateModified;
            UhContext.Addresses.Add(addressLink);
            UhContext.SaveChanges();
            return addressLink;
        }

        private PropertyAlert AddAlertToDatabaseForProperty(int addressNumber, DateTime? endDate = null)
        {
            var alert = _fixture.Build<PropertyAlert>()
                .With(x => x.AddressNumber, addressNumber)
                .With(x => x.EndDate, endDate)
                .Without(x => x.AddressLink)
                .Create();
            UhContext.PropertyAlerts.Add(alert);
            UhContext.SaveChanges();
            return alert;
        }
        private AlertDescriptionLookup AddDescriptionToDatabase(string code)
        {
            var desc = _fixture.Build<AlertDescriptionLookup>()
                .With(d => d.AlertCode, code).Create();

            UhContext.AlertDescriptionLookups.Add(desc);
            UhContext.SaveChanges();
            return desc;
        }
    }
}
