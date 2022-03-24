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
    public class GetPropertyAlertsNewE2ETests : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        [Test]
        public async Task GetPropertyAlertsNewReturnsAlerts()
        {
            // Arrange
            var propertyReference = "00001234";
            var numberOfResults = _random.Next(2, 5);

            var results = _fixture.Build<PropertyAlertNew>()
                .With(x => x.PropertyReference, propertyReference)
                .CreateMany(numberOfResults);

            await TestDataHelper.SavePropertyAlertsToDb(UhContext, results).ConfigureAwait(false);

            // Act
            var response = await GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsPropertyResponse>(data);

            returnedAlerts.PropertyReference.Should().Be(propertyReference);
            returnedAlerts.Alerts.Should().HaveCount(numberOfResults);
        }

        [Test]
        public async Task GetPropertyAlertsNewWhenNoneExistReturnsEmptyList()
        {
            // Arrange
            var propertyReference = "00001234";

            // Act
            var response = await GetPropertyAlertsNew(propertyReference).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsPropertyResponse>(data);

            returnedAlerts.PropertyReference.Should().Be(propertyReference);
            returnedAlerts.Alerts.Should().BeEmpty();
        }

        private async Task<System.Net.Http.HttpResponseMessage> GetPropertyAlertsNew(string propertyReference)
        {
            var url = new Uri($"/api/v1/cautionary-alerts/properties-new/{propertyReference}", UriKind.Relative);

            return await Client.GetAsync(url).ConfigureAwait(true);
        }
    }
}
