using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Factories;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetAlertsForProperty : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public async Task CanRetrieveCautionaryAlertsForProperty()
        {
            var addressLink = TestDataHelper.AddAddressLinkToDb(UhContext, _fixture);
            //alert 1
            var alert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);
            var descAlertOne = TestDataHelper.AddDescriptionToDatabase(UhContext, _fixture, alert.AlertCode);
            //alert2
            var secondAlert = TestDataHelper.AddAlertToDatabaseForProperty(UhContext, _fixture, addressLink.AddressNumber);
            var descAlertTwo = TestDataHelper.AddDescriptionToDatabase(UhContext, _fixture, secondAlert.AlertCode);

            var expectedResponse = new CautionaryAlertsProperty()
            {
                AddressNumber = addressLink.AddressNumber.ToString(),
                PropertyReference = addressLink.PropertyReference,
                UPRN = addressLink.UPRN,
                Alerts = new List<CautionaryAlert>()
                    { alert.ToDomain(descAlertOne.Description), secondAlert.ToDomain(descAlertTwo.Description) }
            }.ToResponse();

            var url = new Uri($"/api/v1/cautionary-alerts/properties/{addressLink.PropertyReference}", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsPropertyResponse>(data);
            returnedAlerts.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public async Task Return404IfPropertyCautionaryAlertsNotFound()
        {
            var url = new Uri($"/api/v1/cautionary-alerts/properties/00123456", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            var errorMessage = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync().ConfigureAwait(true));
            response.StatusCode.Should().Be(404);
            errorMessage.Should().BeEquivalentTo("Property cautionary alert(s) for property reference 00123456 not found");
        }

        [Test]
        public async Task Return404IfPropertyReferenceNotSupplied()
        {
            var url = new Uri($"/api/v1/cautionary-alerts/properties", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(404);
        }
    }
}
