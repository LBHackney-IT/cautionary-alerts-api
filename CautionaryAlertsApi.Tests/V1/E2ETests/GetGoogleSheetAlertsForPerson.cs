using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class GetGoogleSheetAlertsForPerson : IntegrationTests<Startup>
    {
        [Test]
        public async Task CanRetrieveAPersonsCautionaryAlerts()
        {
            var expectedResponse = new CautionaryAlertsGoogleSheetPersonResponse
            {
                PersonId = "566c45c2-1f0c-4ecf-8fbf-afe62d51c8ba",
                Alerts = new List<CautionaryAlertGoogleSheetResponse>
                {
                    new CautionaryAlertGoogleSheetResponse
                    {
                        Code = "CAL", Type = "Caution Type 2", Description = "Caution Description 2"
                    },
                    new CautionaryAlertGoogleSheetResponse
                    {
                        Code = "CAL", Type = "Caution Type 5", Description = "Caution Description 5"
                    }
                }
            };

            var url = new Uri($"/api/v1/cautionary-alerts/sheets/persons/{expectedResponse.PersonId}", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var alerts = JsonConvert.DeserializeObject<CautionaryAlertsGoogleSheetPersonResponse>(data);

            response.StatusCode.Should().Be(200);
            alerts.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
