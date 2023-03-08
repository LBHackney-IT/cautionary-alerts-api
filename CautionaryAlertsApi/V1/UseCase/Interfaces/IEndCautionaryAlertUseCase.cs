using System.Threading.Tasks;
using System;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Core.JWT;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Boundary.Response;

namespace CautionaryAlertsApi.V1.UseCase.Interfaces
{
    public interface IEndCautionaryAlertUseCase
    {
        Task<PropertyAlertDomain> ExecuteAsync(EndCautionaryAlert cautionaryAlert, Token token);
    }
}
