using System;
using System.Threading.Tasks;
using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetAlertsByPersonIdE2ETests : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public async Task GetAlertsByPersonIdReturnsEmptyArrayWhenNoneExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var url = new Uri($"/api/v1/cautionary-alerts/persons/{id}", UriKind.Relative);

            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsMMHPersonResponse>(data);

            returnedAlerts.PersonId.Should().Be(id);
            returnedAlerts.Alerts.Should().BeEmpty();
        }


        [Test]
        public async Task GetAlertsByPersonIdReturnsAlertsIfTheyExist()
        {
            // Arrange
            var id = Guid.NewGuid();

            var alerts = _fixture.Build<PropertyAlertNew>()
                .With(x => x.MMHID, id.ToString())
                .With(x => x.IsActive, true)
                .CreateMany(3);

            alerts.First().IsActive = false;
            await TestDataHelper.SavePropertyAlertsToDb(UhContext, alerts).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{id}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsMMHPersonResponse>(data);

            returnedAlerts.PersonId.Should().Be(id);
            var activAlerts = returnedAlerts.Alerts.FindAll(x => x.IsActive == true);
            activAlerts.Count.Should().Be(2);
        }
    }
}
