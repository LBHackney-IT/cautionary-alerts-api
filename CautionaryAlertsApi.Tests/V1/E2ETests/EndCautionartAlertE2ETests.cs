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
using Amazon.SimpleNotificationService;
using Hackney.Core.Testing.Sns;
using Moq;

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

            alertDb.IsActive = false;

            var url = new Uri($"/api/v1/cautionary-alerts/persons/{personId}/alerts/{alertId}", UriKind.Relative);
            var token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTUwMTgxMTYwOTIwOTg2NzYxMTMiLCJlbWFpbCI6ImUyZS10ZXN0aW5nQGRldmVsb3BtZW50LmNvbSIsImlzcyI6IkhhY2tuZXkiLCJuYW1lIjoiVGVzdGVyIiwiZ3JvdXBzIjpbImUyZS10ZXN0aW5nIl0sImlhdCI6MTYyMzA1ODIzMn0.SooWAr-NUZLwW8brgiGpi2jZdWjyZBwp4GJikn0PvEw";

            var message = new HttpRequestMessage(HttpMethod.Patch, url);

            var jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var requestJson = JsonConvert.SerializeObject(alert, jsonSettings);
            message.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);

            var response = await Client.SendAsync(message).ConfigureAwait(false);
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
