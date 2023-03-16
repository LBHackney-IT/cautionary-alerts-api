using AutoFixture;
using CautionaryAlertsApi.Tests.V1.Helper;
using FluentAssertions;
using Hackney.Core.Testing.Sns;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CautionaryAlertsApi.V1.Domain;
using Hackney.Shared.CautionaryAlerts.Domain;
using Microsoft.IdentityModel.JsonWebTokens;
using Constants = CautionaryAlertsApi.V1.Infrastructure.Constants;
using System.Linq;

namespace CautionaryAlertsApi.Tests.V1.E2ETests
{
    public class EndCautionartAlertE2ETests : IntegrationTests<Startup>
    {
        private readonly Fixture _fixture = new Fixture();
        private ISnsFixture _snsFixture;

        [SetUp]
        public void SetUp()
        {
            _snsFixture = Factory.SnsFixture;
        }

        [Test]
        public async Task EndCautionaryAlertUpdatesIsActiveToFalseAndSendsSns()
        {
            var alertId = Guid.NewGuid();
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));
            var activeAlert = CautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture, addressString);
            var snsMessage = _fixture.Create<CautionaryAlertSns>();

            var alertDb = activeAlert.ToDatabase(isActive: true, alertId.ToString());

            await TestDataHelper.SavePropertyAlertToDb(UhContext, alertDb).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/alerts/{alertId}/end-alert", UriKind.Relative);
            var token = TestsContansts.AuthorizedGroupsToken;

            var message = new HttpRequestMessage(HttpMethod.Patch, url);

            message.Method = HttpMethod.Patch;
            message.Headers.Add("Authorization", token);

            var response = await Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //Sns event sent 
            Action<CautionaryAlertSns> verifyFunc = (snsEvent) =>
            {
                snsEvent.EntityId.Should().Be(alertId);
                snsEvent.User.Name.Should().Be("Tester");
                snsEvent.User.Email.Should().Be("e2e-testing@development.com");
                System.Text.Json.JsonSerializer.Deserialize<PropertyAlertDomain>(
                    snsEvent.EventData.NewData.ToString()
                    ).IsActive.Should().BeFalse();
            };

            var snsVerifer = _snsFixture.GetSnsEventVerifier<CautionaryAlertSns>();
            var snsResult = await snsVerifer.VerifySnsEventRaised(verifyFunc);
            if (!snsResult && snsVerifer.LastException != null)
                throw snsVerifer.LastException;
        }

        [Test]
        public async Task EndCautionaryAlertReturnsNotFoundWhenDoesNotExist()
        {
            var alertId = Guid.NewGuid();
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));

            var alert = CautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture, addressString);

            var url = new Uri($"/api/v1/cautionary-alerts/alerts/{alertId}/end-alert", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, url);

            var token = TestsContansts.AuthorizedGroupsToken;

            message.Headers.Add("Authorization", token);

            var response = await Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task EndCautionaryAlertReturns401UnauthorizedAndDoesNotEndAlertWhenTheTokenDoesNotConainedAllowedGroups()
        {
            var alertId = Guid.NewGuid();
            var defaultString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH));
            var addressString = string.Join("", _fixture.CreateMany<char>(CautionaryAlertConstants.FULLADDRESSLENGTH));
            var activeAlert = CautionaryAlertFixture.GenerateValidCreateCautionaryAlertFixture(defaultString, _fixture, addressString);
            var snsMessage = _fixture.Create<CautionaryAlertSns>();

            var alertDb = activeAlert.ToDatabase(isActive: true, alertId.ToString());

            await TestDataHelper.SavePropertyAlertToDb(UhContext, alertDb).ConfigureAwait(false);

            var url = new Uri($"/api/v1/cautionary-alerts/alerts/{alertId}/end-alert", UriKind.Relative);

            var message = new HttpRequestMessage(HttpMethod.Patch, url);

            var token = TestsContansts.UnauthorizedGroupsToken;

            message.Headers.Add("Authorization", token);

            var response = await Client.SendAsync(message).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var alertInTheDatabase = UhContext.PropertyAlertsNew.FirstOrDefault(a => a.AlertId == alertId.ToString());
            alertInTheDatabase.IsActive.Should().BeTrue();
        }
    }
}
