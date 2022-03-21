using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
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
        private readonly Random _random = new Random();
        private readonly Faker _faker = new Faker();

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new UhGateway(UhContext);
            _fixture = new Fixture();
        }

        [Test]
        public void GetCautionaryAlertsForPeopleReturnsAnEmptyAlertsListIfNoneExistAgainstThePerson()
        {
            var link = AddContactLinkToDb();
            var response =
                _classUnderTest.GetCautionaryAlertsForPeople(link.Key, link.PersonNumber).First();
            response.ContactNumber.Should().Be(link.ContactNumber.ToString());
            response.PersonNumber.Should().Be(link.PersonNumber);
            response.TagRef.Should().Be(link.Key);
            response.Alerts.Should().BeEmpty();
        }

        [Test]
        public void GetCautionaryAlertsForPeopleWillReturnASingleAlert()
        {
            var link = AddContactLinkToDb();
            var alert = AddAlertToDatabaseForContactNumber(link.ContactNumber);
            var desc = AddDescriptionToDatabase(alert.AlertCode);

            var response =
                _classUnderTest.GetCautionaryAlertsForPeople(link.Key, link.PersonNumber).First();

            response.Should().BeEquivalentTo(CompileExpectedResponse(link, alert, desc));
        }

        [Test]
        public void GetCautionaryAlertsForPeopleWillReturnAMultipleAlerts()
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
                _classUnderTest.GetCautionaryAlertsForPeople(link.Key, link.PersonNumber).First();

            response.Should().BeEquivalentTo(new CautionaryAlertPerson
            {
                ContactNumber = link.ContactNumber.ToString(),
                PersonNumber = link.PersonNumber,
                TagRef = link.Key,
                Alerts = expectedAlerts
            });
        }

        [Test]
        public void GetCautionaryAlertsForPeopleReturnsEmptyListIfNoPeopleFound()
        {
            _classUnderTest.GetCautionaryAlertsForPeople("0101/4", "2686").Should().BeEmpty();
        }

        [Test]
        public void GetCautionaryAlertsForPeopleUsesTheMostRecentlyModifiedDescription()
        {
            var link = AddContactLinkToDb();
            var alert = AddAlertToDatabaseForContactNumber(link.ContactNumber);
            var olderDescription = AddDescriptionToDatabase(alert.AlertCode, DateTime.Now.AddMonths(-5));
            var newerDescription = AddDescriptionToDatabase(alert.AlertCode, DateTime.Now.AddMonths(-2));

            var response =
                _classUnderTest.GetCautionaryAlertsForPeople(link.Key, link.PersonNumber).First();

            response.Should().BeEquivalentTo(CompileExpectedResponse(link, alert, newerDescription));
        }

        [Test]
        public void GetCautionaryAlertsForPeopleReturnsAlertsAgainstAnyMatchingContactLinkRecord()
        {
            var olderLink = AddContactLinkToDb(dateModified: DateTime.Now.AddMonths(-11));
            var olderAlert = AddAlertToDatabaseForContactNumber(olderLink.ContactNumber);
            var olderDesc = AddDescriptionToDatabase(olderAlert.AlertCode);

            var newerLink = AddContactLinkToDb(olderLink.Key, olderLink.PersonNumber, DateTime.Now.AddMonths(-3));
            var newerAlert = AddAlertToDatabaseForContactNumber(newerLink.ContactNumber);
            var newerDesc = AddDescriptionToDatabase(newerAlert.AlertCode);


            var response =
                _classUnderTest.GetCautionaryAlertsForPeople(newerLink.Key, newerLink.PersonNumber);

            response.Should().BeEquivalentTo(new List<CautionaryAlertPerson>{
                    CompileExpectedResponse(newerLink, newerAlert, newerDesc),
                    CompileExpectedResponse(olderLink, olderAlert, olderDesc)
            });
        }

        [Test]
        public void GetCautionaryAlertsForPeopleWontDuplicateAlertsWhenContactLinkExistsForHouseAndTenancyRef()
        {
            var link = AddContactLinkToDb();
            var duplicateLink = new ContactLink
            {
                Key = link.Key.Remove(0, 2),
                ContactNumber = link.ContactNumber,
                DateModified = link.DateModified,
                LinkNumber = link.LinkNumber + 1,
                LinkType = "Current Tenancy",
                ModifyType = "I",
                PersonNumber = link.PersonNumber
            };
            UhContext.ContactLinks.Add(duplicateLink);
            var alert = AddAlertToDatabaseForContactNumber(link.ContactNumber);
            AddDescriptionToDatabase(alert.AlertCode);

            var response =
                _classUnderTest.GetCautionaryAlertsForPeople(link.Key, link.PersonNumber);

            response.First().Alerts.Count.Should().Be(1);
        }

        [Test]
        public void GetCautionaryAlertsForAllPeopleInATenancy()
        {
            var tenancyRef = _faker.Random.AlphaNumeric(10);
            var expectedAlerts = new List<CautionaryAlert>();
            var linkOne = AddContactLinkToDb(tenancyRef, "1");
            var linkTwo = AddContactLinkToDb(tenancyRef, "2");

            var alertForLinkOne = AddAlertToDatabaseForContactNumber(linkOne.ContactNumber);
            var alertOneDesc = AddDescriptionToDatabase(alertForLinkOne.AlertCode);
            expectedAlerts.Add(alertForLinkOne.ToDomain(alertOneDesc.Description));

            var secondAlertForLinkOne = AddAlertToDatabaseForContactNumber(linkOne.ContactNumber);
            var secondAlertDesc = AddDescriptionToDatabase(secondAlertForLinkOne.AlertCode);
            expectedAlerts.Add(secondAlertForLinkOne.ToDomain(secondAlertDesc.Description));


            var alertForLinkTwo = AddAlertToDatabaseForContactNumber(linkTwo.ContactNumber);
            var thirdAlertDesc = AddDescriptionToDatabase(alertForLinkTwo.AlertCode);

            var response =
              _classUnderTest.GetCautionaryAlertsForPeople(tenancyRef, "");

            response.Should().BeEquivalentTo(new List<CautionaryAlertPerson>{
                    new CautionaryAlertPerson
                    {
                        ContactNumber = linkOne.ContactNumber.ToString(),
                        PersonNumber = linkOne.PersonNumber,
                        TagRef = linkOne.Key,
                        Alerts = expectedAlerts
                    }, new CautionaryAlertPerson
                    {
                        ContactNumber = linkTwo.ContactNumber.ToString(),
                        PersonNumber = linkTwo.PersonNumber,
                        TagRef = linkTwo.Key,
                        Alerts = new List<CautionaryAlert>
                        {
                            alertForLinkTwo.ToDomain(thirdAlertDesc.Description)
                        }
                    }
            });
        }
        [Test]
        public void GetCautionaryAlertsForPeopleInTenancyReturnsBothContactsEvenIfOneDoesNotHaveAlerts()
        {
            var tenancyRef = _faker.Random.AlphaNumeric(10);
            var expectedAlerts = new List<CautionaryAlert>();
            var linkOne = AddContactLinkToDb(tenancyRef, "1");
            var linkTwo = AddContactLinkToDb(tenancyRef, "2");

            var alertForLinkOne = AddAlertToDatabaseForContactNumber(linkOne.ContactNumber);
            var alertOneDesc = AddDescriptionToDatabase(alertForLinkOne.AlertCode);
            expectedAlerts.Add(alertForLinkOne.ToDomain(alertOneDesc.Description));

            var secondAlertForLinkOne = AddAlertToDatabaseForContactNumber(linkOne.ContactNumber);
            var secondAlertDesc = AddDescriptionToDatabase(secondAlertForLinkOne.AlertCode);
            expectedAlerts.Add(secondAlertForLinkOne.ToDomain(secondAlertDesc.Description));

            var response =
              _classUnderTest.GetCautionaryAlertsForPeople(tenancyRef, "");

            response.Should().BeEquivalentTo(new List<CautionaryAlertPerson>{
                    new CautionaryAlertPerson
                    {
                        ContactNumber = linkOne.ContactNumber.ToString(),
                        PersonNumber = linkOne.PersonNumber,
                        TagRef = linkOne.Key,
                        Alerts = expectedAlerts
                    }, new CautionaryAlertPerson
                    {
                        ContactNumber = linkTwo.ContactNumber.ToString(),
                        PersonNumber = linkTwo.PersonNumber,
                        TagRef = linkTwo.Key,
                        Alerts = new List<CautionaryAlert>()
                    }
            });
        }
        [Test]
        public void GetCautionaryAlertsForPeopleWithoutSupplyingPersonNoReturnsCorrectContact()
        {
            var tenancyRef = _faker.Random.AlphaNumeric(10);
            var linkOne = AddContactLinkToDb(tenancyRef, "1");
            var linkTwo = AddContactLinkToDb();

            var alertForLinkOne = AddAlertToDatabaseForContactNumber(linkOne.ContactNumber);
            var alertOneDesc = AddDescriptionToDatabase(alertForLinkOne.AlertCode);

            var alertForLinkTwo = AddAlertToDatabaseForContactNumber(linkTwo.ContactNumber);
            var alertOTwoDesc = AddDescriptionToDatabase(alertForLinkTwo.AlertCode);

            var response =
              _classUnderTest.GetCautionaryAlertsForPeople(tenancyRef, "");

            response.Count.Should().Be(1);
            response.First().Alerts.Count.Should().Be(1);
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

            var expectedResponse = new CautionaryAlertsProperty()
            {
                AddressNumber = addressLink.AddressNumber.ToString(),
                PropertyReference = addressLink.PropertyReference,
                UPRN = addressLink.UPRN
            };
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

        [Test]
        public async Task GetCautionaryContactsWhenNoneExistReturnsEmptyList()
        {
            // Arrange
            var propertyReference = "00001234";

            // Act
            var result = await _classUnderTest.GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetCautionaryContactsWhenCalledReturnsMany()
        {
            // Arrange
            var propertyReference = "00001234";
            var numberOfResults = _random.Next(2, 5);

            var results = _fixture.Build<PropertyAlertsNew>()
                .With(x => x.PropertyReference, propertyReference)
                .CreateMany(numberOfResults);

            await SaveCautionaryContactsToDb(results).ConfigureAwait(false);

            // Act
            var result = await _classUnderTest.GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(numberOfResults);
        }

        private async Task SaveCautionaryContactsToDb(IEnumerable<PropertyAlertsNew> results)
        {
            UhContext.PropertyAlertsNew.AddRange(results);
            await UhContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
