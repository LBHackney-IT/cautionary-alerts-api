using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

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
        public void GetCautionaryAlertsForAPersonReturnsAnEmptyAlertsListIfNoneExistAgainstThePerson()
        {
            var link = AddContactLinkToDb();
            var response =
                _classUnderTest.GetCautionaryAlertsForAPerson(link.Key, link.PersonNumber).First();
            response.ContactNumber.Should().Be(link.ContactNumber.ToString());
            response.PersonNumber.Should().Be(link.PersonNumber);
            response.TagRef.Should().Be(link.Key);
            response.Alerts.Should().BeEmpty();
        }

        [Test]
        public void GetCautionaryAlertsForAPersonWillReturnASingleAlert()
        {
            var link = AddContactLinkToDb();
            var alert = AddAlertToDatabaseForContactNumber(link.ContactNumber);
            var desc = AddDescriptionToDatabase(alert.AlertCode);

            var response =
                _classUnderTest.GetCautionaryAlertsForAPerson(link.Key, link.PersonNumber).First();

            response.Should().BeEquivalentTo(CompileExpectedResponse(link, alert, desc));
        }

        [Test]
        public void GetCautionaryAlertsForAPersonWillReturnAMultipleAlerts()
        {
            var link = AddContactLinkToDb();
            var expectedAlerts = new List<CautionaryAlert>();
            foreach (var i in Enumerable.Range(0, 3))
            {
                var alert = AddAlertToDatabaseForContactNumber(link.ContactNumber);
                var desc = AddDescriptionToDatabase(alert.AlertCode);
                expectedAlerts.Add(alert.ToDomain(desc.Description));
            }

            var response =
                _classUnderTest.GetCautionaryAlertsForAPerson(link.Key, link.PersonNumber).First();

            response.Should().BeEquivalentTo(new CautionaryAlertPerson
            {
                ContactNumber = link.ContactNumber.ToString(),
                PersonNumber = link.PersonNumber,
                TagRef = link.Key,
                Alerts = expectedAlerts
            });
        }

        [Test]
        public void GetCautionaryAlertsForAPersonReturnsEmptyListIfNoPeopleFound()
        {
            _classUnderTest.GetCautionaryAlertsForAPerson("0101/4", "2686").Should().BeEmpty();
        }

        [Test]
        public void GetCautionaryAlertsForAPersonUsesTheMostRecentlyModifiedDescription()
        {
            var link = AddContactLinkToDb();
            var alert = AddAlertToDatabaseForContactNumber(link.ContactNumber);
            var olderDescription = AddDescriptionToDatabase(alert.AlertCode, DateTime.Now.AddMonths(-5));
            var newerDescription = AddDescriptionToDatabase(alert.AlertCode, DateTime.Now.AddMonths(-2));

            var response =
                _classUnderTest.GetCautionaryAlertsForAPerson(link.Key, link.PersonNumber).First();

            response.Should().BeEquivalentTo(CompileExpectedResponse(link, alert, newerDescription));
        }

        [Test]
        public void GetCautionaryAlertsForAPersonReturnsAlertsAgainstAnyMatchingContactLinkRecord()
        {
            var olderLink = AddContactLinkToDb(dateModified: DateTime.Now.AddMonths(-11));
            var olderAlert = AddAlertToDatabaseForContactNumber(olderLink.ContactNumber);
            var olderDesc = AddDescriptionToDatabase(olderAlert.AlertCode);

            var newerLink = AddContactLinkToDb(olderLink.Key, olderLink.PersonNumber, DateTime.Now.AddMonths(-3));
            var newerAlert = AddAlertToDatabaseForContactNumber(newerLink.ContactNumber);
            var newerDesc = AddDescriptionToDatabase(newerAlert.AlertCode);


            var response =
                _classUnderTest.GetCautionaryAlertsForAPerson(newerLink.Key, newerLink.PersonNumber);

            response.Should().BeEquivalentTo(new List<CautionaryAlertPerson>{
                    CompileExpectedResponse(newerLink, newerAlert, newerDesc),
                    CompileExpectedResponse(olderLink, olderAlert, olderDesc)
            });
        }

        private static CautionaryAlertPerson CompileExpectedResponse(ContactLink link, PersonAlert alert, AlertDescriptionLookup description)
        {
            return new CautionaryAlertPerson
            {
                ContactNumber = link.ContactNumber.ToString(),
                PersonNumber = link.PersonNumber,
                TagRef = link.Key,
                Alerts = new List<CautionaryAlert>
                {
                    alert.ToDomain(description.Description)
                }
            };
        }

        private ContactLink AddContactLinkToDb(string tagRef = null, string personNo = null, DateTime? dateModified = null)
        {
            var contactLink = _fixture.Create<ContactLink>();
            contactLink.Key = tagRef ?? contactLink.Key;
            contactLink.PersonNumber = personNo ?? contactLink.PersonNumber;
            contactLink.DateModified = dateModified ?? contactLink.DateModified;
            UhContext.ContactLinks.Add(contactLink);
            UhContext.SaveChanges();
            return contactLink;
        }

        private PersonAlert AddAlertToDatabaseForContactNumber(int contactNumber)
        {
            var alert = _fixture.Build<PersonAlert>()
                .With(a => a.ContactNumber, contactNumber)
                .Without(a => a.ContactLink)
                .Create();
            UhContext.PeopleAlerts.Add(alert);
            UhContext.SaveChanges();
            return alert;
        }

        private AlertDescriptionLookup AddDescriptionToDatabase(string code, DateTime? dateModified = null)
        {
            var desc = _fixture.Build<AlertDescriptionLookup>()
                .With(d => d.AlertCode, code).Create();

            desc.DateModified = dateModified ?? desc.DateModified;

            UhContext.AlertDescriptionLookups.Add(desc);
            UhContext.SaveChanges();
            return desc;
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
            var addressLink = TestDataHelper.AddAddressLinkToDb(UhContext, _fixture);
            //alert 1
            var alert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);
            var descAlertOne = AddDescriptionToDatabase(alert.AlertCode);
            //alert2
            var secondAlert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);
            var descAlertTwo = AddDescriptionToDatabase(secondAlert.AlertCode);

            var expectedResponse = new CautionaryAlertsProperty() { AddressNumber = addressLink.AddressNumber.ToString(), PropertyReference = addressLink.PropertyReference,
            UPRN = addressLink.UPRN };
            expectedResponse.Alerts = new List<CautionaryAlert>() { alert.ToDomain(descAlertOne.Description), secondAlert.ToDomain(descAlertTwo.Description) };

            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.Should().BeEquivalentTo(expectedResponse);
            response.Alerts.Count.Should().Be(2);
        }

        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAlertsWithDescriptions()
        {
            var addressLink = TestDataHelper.AddAddressLinkToDb(UhContext, _fixture);
            var alert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);
            var descAlertOne = AddDescriptionToDatabase(alert.AlertCode);

            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.Alerts.First().Description.Should().BeEquivalentTo(descAlertOne.Description);
        }

        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAddressNumberUPRNAndPropertyReferenceForProperty()
        {
            var addressLink = TestDataHelper.AddAddressLinkToDb(UhContext, _fixture);
            var alert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);

            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.AddressNumber.Should().BeEquivalentTo(addressLink.AddressNumber.ToString());
            response.PropertyReference.Should().BeEquivalentTo(addressLink.PropertyReference.ToString());
            response.UPRN.Should().BeEquivalentTo(addressLink.UPRN.ToString());
        }


        [Test]
        public void GetCautionaryAlertsForPropertyReturnsAlertsInformation()
        {
            var addressLink = TestDataHelper.AddAddressLinkToDb(UhContext, _fixture);
            var alert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);
            var response =
                _classUnderTest.GetCautionaryAlertsForAProperty(addressLink.PropertyReference);

            response.Alerts.First().StartDate.Should().BeSameDateAs(alert.StartDate);
            response.Alerts.First().DateModified.Should().BeSameDateAs(alert.DateModified);
            response.Alerts.First().EndDate.Should().BeNull();
            response.Alerts.First().ModifiedBy.Should().BeEquivalentTo(alert.ModifiedBy);
            response.Alerts.First().AlertCode.Should().BeEquivalentTo(alert.AlertCode);
        }
    }
}
