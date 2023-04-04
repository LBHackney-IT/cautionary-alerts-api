using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using System.Threading.Tasks;
using System;
using CautionaryAlertsApi.V1.Factories;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.CautionaryAlerts.Domain;
using CautionaryAlertsApi.V1.Boundary.Request;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class EndCautionaryAlertUseCase : IEndCautionaryAlertUseCase
    {
        private readonly IUhGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public EndCautionaryAlertUseCase(IUhGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }
        public async Task<PropertyAlertDomain> ExecuteAsync(AlertQueryObject query, EndCautionaryAlertRequest endCautionaryAlertRequest, Token token)
        {
            var updatedAlert = await _gateway.EndCautionaryAlert(query, endCautionaryAlertRequest).ConfigureAwait(false);

            if (updatedAlert is null)
                return null;

            var cautionaryAlertSnsMessage = _snsFactory.End(updatedAlert, token);
            var cautionaryAlertTopicArn = Environment.GetEnvironmentVariable("CAUTIONARY_ALERTS_SNS_ARN");

            await _snsGateway.Publish(cautionaryAlertSnsMessage, cautionaryAlertTopicArn).ConfigureAwait(false);

            return updatedAlert;
        }
    }
}
