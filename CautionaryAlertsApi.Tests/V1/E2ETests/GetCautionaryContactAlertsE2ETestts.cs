using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetCautionaryContactAlertsE2ETests : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();

        [Test]
        public async Task CanGetCautionaryContactAlerts()
        {
            // Arrange
            var propertyReference = "00001234";
            var numberOfResults = _random.Next(2, 5);

            var results = _fixture.Build<CautionaryContact>()
                .With(x => x.PropertyReference, propertyReference)
                .CreateMany(numberOfResults);

            await SaveCautionaryContactsToDb(results).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/properties-new/{propertyReference}", UriKind.Relative);

            // Act
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedAlerts = JsonConvert.DeserializeObject<CautionaryAlertsPropertyResponse>(data);

            returnedAlerts.PropertyReference.Should().Be(propertyReference);
            returnedAlerts.Alerts.Should().HaveCount(numberOfResults);
        }

        private async Task SaveCautionaryContactsToDb(IEnumerable<CautionaryContact> results)
        {
            UhContext.CautionaryContacts.AddRange(results);
            await UhContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
