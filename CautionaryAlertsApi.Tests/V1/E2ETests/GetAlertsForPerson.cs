using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetAlertsForPerson : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public async Task  CanRetrieveAPersonsCautionaryAlerts()
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

            var expectedResponse = new CautionaryAlertPersonResponse
            {
                ContactNumber = link.ContactNumber.ToString(),
                PersonNumber = link.PersonNumber,
                TagRef = link.Key,
                Alerts = expectedAlertsResponse
            };

            var url = new Uri($"/tag-ref/{link.Key}/person-number/{link.PersonNumber}", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertPersonResponse>(data);
            returnedAlerts.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public async Task IfThePersonCantBeLocatedReturnsA404()
        {
            var url = new Uri($"/tag-ref/1236735%2F01/person-number/6376c", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(404);
        }
        private ContactLink AddContactLinkToDb()
        {
            var contactLink = _fixture.Create<ContactLink>();
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
