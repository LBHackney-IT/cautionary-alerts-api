using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using Hackney.Core.JWT;
using Hackney.Core.Sns;
using System;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using CautionaryAlertsApi.V1.Factories;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Domain;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class PostNewCautionaryAlertUseCase : IPostNewCautionaryAlertUseCase
    {
        private readonly IUhGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public PostNewCautionaryAlertUseCase(IUhGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<CautionaryAlertListItem> ExecuteAsync(CreateCautionaryAlert createCautionaryAlert, Token token)
        {
            var cautionaryAlert = await _gateway.PostNewCautionaryAlert(createCautionaryAlert).ConfigureAwait(false);

            var cautionaryAlertSnsMessage = _snsFactory.Create(cautionaryAlert, token);
            var cautionaryAlertTopicArn = Environment.GetEnvironmentVariable("CAUTIONARY_ALERTS_SNS_ARN");

            await _snsGateway.Publish(cautionaryAlertSnsMessage, cautionaryAlertTopicArn).ConfigureAwait(false);

            return cautionaryAlert.ToResponse();
        }
    }
}
