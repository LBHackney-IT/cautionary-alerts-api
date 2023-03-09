using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using FluentAssertions;
using Hackney.Core.JWT;
using Hackney.Core.Middleware;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;
using System.Net;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class EndCautionartAlertE2ETests : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]

        public async Task EndCautionaryAlertUpdatesIsActiveToFalse()
        {
            var personId = Guid.NewGuid();
            var alertId = Guid.NewGuid();
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));

            var alert = CautionaryAlertFixture.GenerateValidEndCautionaryAlertFixture(personId, alertId, defaultString, addressString, _fixture);

            var alertDb = alert.ToDatabase();

            await TestDataHelper.SavePropertyAlertToDb(UhContext, alertDb).ConfigureAwait(false);

            alert.IsActive = false;

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{personId}/alerts/{alertId}", UriKind.Relative);

            var content = new StringContent(JsonConvert.SerializeObject(alert), Encoding.UTF8, "application/json");

            var response = await Client.PatchAsync(url, content).ConfigureAwait(false);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Test]

        public async Task EndCautionaryAlertReturnsNotFoundWhenDoesNotExist()
        {
            var personId = Guid.NewGuid();
            var alertId = Guid.NewGuid();
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));

            var alert = CautionaryAlertFixture.GenerateValidEndCautionaryAlertFixture(personId, alertId, defaultString, addressString, _fixture);

            alert.IsActive = false;

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{personId}/alerts/{alertId}", UriKind.Relative);

            var content = new StringContent(JsonConvert.SerializeObject(alert), Encoding.UTF8, "application/json");

            var response = await Client.PatchAsync(url, content).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var deserialize = JsonConvert.DeserializeObject<PropertyAlertDomain>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            
        }
    }
}
