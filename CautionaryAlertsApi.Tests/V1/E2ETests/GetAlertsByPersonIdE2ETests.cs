using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

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
                .CreateMany();
            await TestDataHelper.SavePropertyAlertsToDb(UhContext, alerts).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{id}", UriKind.Relative);
            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsMMHPersonResponse>(data);

            returnedAlerts.PersonId.Should().Be(id);
            returnedAlerts.Alerts.Should().HaveSameCount(alerts);
        }
    }
}
