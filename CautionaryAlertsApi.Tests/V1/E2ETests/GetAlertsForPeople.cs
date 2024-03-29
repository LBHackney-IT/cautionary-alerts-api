using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetAlertsForPeople : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public async Task CanRetrieveAPersonsCautionaryAlerts()
        {
            var link = AddContactLinkToDb();
            var alerts = new List<PersonAlert>
            {
                AddAlertToDatabaseForContactNumber(link.ContactNumber),
                AddAlertToDatabaseForContactNumber(link.ContactNumber)
            };
            var expectedAlertsResponse = alerts.Select(a =>
            {
                var desc = AddDescriptionToDatabase(a.AlertCode);
                return a.ToDomain(desc.Description).ToResponse();
            }).ToList();

            var expectedResponse = new ListPersonsCautionaryAlerts
            {
                Contacts = new List<CautionaryAlertPersonResponse>{
                    new CautionaryAlertPersonResponse
                    {
                        ContactNumber = link.ContactNumber.ToString(),
                        PersonNumber = link.PersonNumber,
                        TenancyAgreementReference = link.Key,
                        Alerts = expectedAlertsResponse
                    }
                }
            };

            var url = new Uri($"/api/v1/cautionary-alerts/people?tag_ref={link.Key}&person_number={link.PersonNumber}", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<ListPersonsCautionaryAlerts>(data);
            returnedAlerts.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public async Task IfThePersonCantBeLocatedReturnsA404()
        {
            var url = new Uri("/api/v1/cautionary-alerts/people?tag_ref=1236735/01&person_number=6376c", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task IfTagRefIsNotAQueryParameterReturnsA400()
        {
            var url = new Uri("/api/v1/cautionary-alerts/people?person_number=6376c", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(400);
        }
        [Test]
        public async Task CanRetrieveCautionaryAlertsForAllPeopleInATenancy()
        {
            var linkOne = AddContactLinkToDb();
            var linkTwo = AddContactLinkToDb(linkOne.Key);


            var linkOneAlert = AddAlertToDatabaseForContactNumber(linkOne.ContactNumber);
            var alertOneDesc = AddDescriptionToDatabase(linkOneAlert.AlertCode);

            var linkTwoAlert = AddAlertToDatabaseForContactNumber(linkTwo.ContactNumber);
            var alertTwoDesc = AddDescriptionToDatabase(linkTwoAlert.AlertCode);

            var expectedResponse = new ListPersonsCautionaryAlerts
            {
                Contacts = new List<CautionaryAlertPersonResponse>{
                    new CautionaryAlertPersonResponse
                    {
                        ContactNumber = linkOne.ContactNumber.ToString(),
                        PersonNumber = linkOne.PersonNumber,
                        TenancyAgreementReference = linkOne.Key,
                        Alerts = new List<CautionaryAlertResponse> { linkOneAlert.ToDomain(alertOneDesc.Description).ToResponse()}

                    },
                new CautionaryAlertPersonResponse
                {
                    ContactNumber = linkTwo.ContactNumber.ToString(),
                    PersonNumber = linkTwo.PersonNumber,
                    TenancyAgreementReference = linkTwo.Key,
                    Alerts = new List<CautionaryAlertResponse> { linkTwoAlert.ToDomain(alertTwoDesc.Description).ToResponse() }
                }
            }
            };

            var url = new Uri($"/api/v1/cautionary-alerts/people?tag_ref={linkOne.Key}", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<ListPersonsCautionaryAlerts>(data);
            returnedAlerts.Should().BeEquivalentTo(expectedResponse);
        }

        private ContactLink AddContactLinkToDb(string tagRef = null)
        {
            var contactLink = _fixture.Create<ContactLink>();
            contactLink.Key = tagRef ?? contactLink.Key;

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
