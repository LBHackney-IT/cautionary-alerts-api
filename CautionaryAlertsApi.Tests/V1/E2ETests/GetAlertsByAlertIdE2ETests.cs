using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using FluentAssertions;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetAlertsByAlertIdE2ETests : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();


        [Test]
        public async Task GetAlertsByAlertIdReturnsAlert()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var alertId = Guid.NewGuid();
            var dateOfIncident = "12/12/2020";

            var alerts = _fixture.Build<PropertyAlertNew>()
                .With(x => x.MMHID, personId.ToString())
                .With(x => x.AlertId, alertId.ToString())
                .With(x => x.DateOfIncident, dateOfIncident)
                .Create();

            await TestDataHelper.SavePropertyAlertToDb(UhContext, alerts).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{personId}/alerts/{alertId}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertResponse>(data);

            returnedAlerts.PersonId.Should().Be(personId);
            returnedAlerts.AlertId.Should().Be(alertId);
        }

        [Test]
        public async Task ReturnsNotFoundWhenAlertDoesNotExist()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var alertId = Guid.NewGuid();

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{personId}/alerts/{alertId}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task Returns500WhenMoreThanOneAlertRetrievedFromDb()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var alertId = Guid.NewGuid();
            var dateOfIncident = "12/12/2020";

            var alerts = _fixture.Build<PropertyAlertNew>()
                .With(x => x.MMHID, personId.ToString())
                .With(x => x.AlertId, alertId.ToString())
                .With(x => x.DateOfIncident, dateOfIncident)
                .CreateMany();

            await TestDataHelper.SavePropertyAlertsToDb(UhContext, alerts).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{personId}/alerts/{alertId}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(500);
        }
    }
}
