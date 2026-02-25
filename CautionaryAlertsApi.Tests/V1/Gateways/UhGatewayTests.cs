using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Bogus;
using CautionaryAlertsApi.Tests.V1.Helper;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Factories;
using CautionaryAlertsApi.V1.Gateways;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using CautionaryAlertsApi.Tests.V1.Infrastructure;
using CautionaryAlertsApi.V1.Boundary.Request;

namespace CautionaryAlertsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class UhGatewayTests : DatabaseTests
    {
        private UhGateway _classUnderTest;

        private Fixture _fixture;
        private readonly Random _random = new Random();
        private readonly Faker _faker = new Faker();
        private Mock<ILogger<UhGateway>> _mockedLogger;


        [SetUp]
        public void Setup()
        {
            new LogCallAspectFixture().RunBeforeTests();
            _mockedLogger = new Mock<ILogger<UhGateway>>();
            _classUnderTest = new UhGateway(UhContext, _mockedLogger.Object);
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
        public async Task GetPropertyAlertsWhenNoneExistReturnsEmptyList()
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
        public async Task GetPropertyAlertsWhenCalledReturnsMany()
        {
            // Arrange
            var propertyReference = "00001234";
            var numberOfResults = _random.Next(2, 5);

            var results = _fixture.Build<PropertyAlertNew>()
                .With(x => x.PropertyReference, propertyReference)
                .With(x => x.IsActive, true)
                .CreateMany(numberOfResults);

            await TestDataHelper.SavePropertyAlertsToDb(UhContext, results).ConfigureAwait(false);

            // Act
            var result = await _classUnderTest.GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(numberOfResults);
        }

        [Test]
        public async Task GetPropertyAlertsReturnsEmptyWhenAllInActive()
        {
            // Arrange
            var propertyReference = "00001234";
            var numberOfResults = _random.Next(2, 5);

            var results = _fixture.Build<PropertyAlertNew>()
                .With(x => x.PropertyReference, propertyReference)
                .With(x => x.IsActive, false)
                .CreateMany(numberOfResults);

            await TestDataHelper.SavePropertyAlertsToDb(UhContext, results).ConfigureAwait(false);

            // Act
            var result = await _classUnderTest.GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetCautionaryAlertsByPersonIdReturnsEmptyList()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var results = _fixture.Build<PropertyAlertNew>()
                                  .With(x => x.MMHID, personId.ToString())
                                  .Create();


            // Act
            var result = await _classUnderTest.GetCautionaryAlertsByMMHPersonId(personId).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetCautionaryAlertsByPersonIdReturnsMany()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var results = _fixture.Build<PropertyAlertNew>()
                .With(x => x.MMHID, personId.ToString())
                .With(x => x.IsActive, true)
                .CreateMany(3);

            results.First().IsActive = false;
            await TestDataHelper.SavePropertyAlertsToDb(UhContext, results).ConfigureAwait(false);

            // Act
            var result = await _classUnderTest.GetCautionaryAlertsByMMHPersonId(personId).ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            var activAlerts = result.Select(x => x.IsActive == true);
            activAlerts.Should().HaveCount(2);
        }

        [Test]
        public async Task GetCautionaryAlertsByAlertIdReturnsAlert()
        {
            // Arrange
            var query = _fixture.Create<AlertQueryObject>();

            var dateOfIncident = "12/12/2020";
            var alert = _fixture.Build<PropertyAlertNew>()
                .With(x => x.AlertId, query.AlertId.ToString())
                .With(x => x.DateOfIncident, dateOfIncident)
                .Without(x => x.EndDate)
                .Create();

            await TestDataHelper.SavePropertyAlertToDb(UhContext, alert).ConfigureAwait(false);

            // Act
            var result = _classUnderTest.GetCautionaryAlertByAlertId(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(alert.ToPropertyAlertDomain(), config => config.Excluding(x => x.EndDate));
            // The end date should be null as isActive is true. 
            result.EndDate.Should().BeNull();
        }

        [Test]
        public void GetCautionaryAlertsByAlertIdReturnsNull()
        {
            // Arrange
            var query = _fixture.Create<AlertQueryObject>();

            var dateOfIncident = "12/12/2020";
            var alert = _fixture.Build<PropertyAlertNew>()
                .With(x => x.AlertId, query.AlertId.ToString())
                .With(x => x.DateOfIncident, dateOfIncident)
                .Create();

            // Act
            var result = _classUnderTest.GetCautionaryAlertByAlertId(query);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetCautionaryAlertsByAlertIdThrowsException()
        {
            // Arrange
            var query = _fixture.Create<AlertQueryObject>();

            var dateOfIncident = "12/12/2020";
            var alerts = _fixture.Build<PropertyAlertNew>()
                .With(x => x.AlertId, query.AlertId.ToString())
                .With(x => x.DateOfIncident, dateOfIncident)
                .CreateMany(3);

            await TestDataHelper.SavePropertyAlertsToDb(UhContext, alerts).ConfigureAwait(false);

            // Act
            Func<PropertyAlertDomain> func = () => _classUnderTest.GetCautionaryAlertByAlertId(query);

            // Assert
            func.Should().Throw<MoreThanOneAlertException>().Where(x => x.AlertCount > 1);

        }

        [Test]
        public async Task PostNewCautionaryAlertReturnsEntityIfSuccessful()
        {
            // Arrange
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));
            var cautionaryAlert = CautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture, addressString);

            // Act
            var response = await _classUnderTest.PostNewCautionaryAlert(cautionaryAlert).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<PropertyAlertDomain>();
            response.Reason.Should().BeSameAs(cautionaryAlert.IncidentDescription);
            response.Code.Should().BeSameAs(cautionaryAlert.Alert.Code);
            response.PropertyReference.Should().BeSameAs(cautionaryAlert.AssetDetails.PropertyReference);
        }

        [Test]
        public async Task PostNewCautionaryAlertWithoutAssetDetailsReturnsEntityIfSuccessful()
        {
            // Arrange
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var cautionaryAlert = CautionaryAlertFixture.GenerateValidCreateCautionaryAlertWithoutAssetDetailsFixture(defaultString, _fixture);

            // Act
            var response = await _classUnderTest.PostNewCautionaryAlert(cautionaryAlert).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<PropertyAlertDomain>();
            response.Reason.Should().BeSameAs(cautionaryAlert.IncidentDescription);
            response.Code.Should().BeSameAs(cautionaryAlert.Alert.Code);
        }

        [Test]
        public void PostNewCautionaryAlertThrowsIfNotSuccessful()
        {
            // Arrange
            var cautionaryAlert = _fixture.Create<CreateCautionaryAlert>();

            // Act & Assert
            Assert.ThrowsAsync<DbUpdateException>(async () => await _classUnderTest.PostNewCautionaryAlert(cautionaryAlert).ConfigureAwait(false));
        }

        [Test]
        public async Task EndCautionaryAlertReturnsEntityIfSuccessful()
        {
            // Arrange
            var alertId = Guid.NewGuid();
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));

            var createAlert = CautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture, addressString);

            var alertDb = createAlert.ToDatabase(isActive: true, alertId.ToString());

            alertDb.Id = _fixture.Create<int>();

            await TestDataHelper.SavePropertyAlertToDb(UhContext, alertDb).ConfigureAwait(false);

            var endCautionaryAlertData = _fixture.Build<AlertQueryObject>().With(x => x.AlertId, alertId).Create();
            var endCautionaryAlertRequest = _fixture.Build<EndCautionaryAlertRequest>()
                                                    .With(x => x.EndDate, DateTime.UtcNow.AddYears(-1))
                                                    .Create();

            // Act
            var response = await _classUnderTest.EndCautionaryAlert(endCautionaryAlertData, endCautionaryAlertRequest).ConfigureAwait(false);

            // Assert
            response.Should().NotBeNull();

            //Ensures it updates record and doesn't create a new one.
            var updatedAlert = UhContext.PropertyAlertsNew
                                        .Where(x => x.AlertId == alertId.ToString());
            updatedAlert.Count().Should().Be(1);

            var originalAlertDomain = alertDb.ToPropertyAlertDomain();
            var updatedAlertDomain = updatedAlert.Select(x => x.ToPropertyAlertDomain()).ToList().FirstOrDefault();

            updatedAlertDomain.Should().BeEquivalentTo(originalAlertDomain, config => config.Excluding(x => x.IsActive).Excluding(y => y.EndDate));
            updatedAlert.FirstOrDefault().IsActive.Should().BeFalse();
        }
    }
}
