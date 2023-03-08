using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;
using System.Threading.Tasks;
using System;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using CautionaryAlertsApi.V1.Factories;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Domain;

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
        public async Task<CautionaryAlertResponse> ExecuteAsync(EndCautionaryAlert cautionaryAlert, Token token)
        {
             var result = await _gateway.EndCautionaryAlert(cautionaryAlert).ConfigureAwait(false);

            var cautionaryAlertSnsMessage = _snsFactory.End(result, token);
            var cautionaryAlertTopicArn = Environment.GetEnvironmentVariable("CAUTIONARY_ALERTS_SNS_ARN");

            await _snsGateway.Publish(cautionaryAlertSnsMessage, cautionaryAlertTopicArn).ConfigureAwait(false);

            return result.CautionaryAlertToResponse();

        }
    }
}
