using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Gateways;
using CautionaryAlertsApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.UseCase
{
    public class GetCautionaryAlertsForProperty : IGetCautionaryAlertsForProperty
    {
        private IUhGateway _gateway;
        public GetCautionaryAlertsForProperty(IUhGateway gateway)
        {
            _gateway = gateway;
        }
        public CautionaryAlertsPropertyResponse Execute(string propertyReference)
        {
            var response = _gateway.GetCautionaryAlertsForAProperty(propertyReference);

            if (response == null || (!response.Alerts?.Any() ?? true))
            {
                throw new PropertyAlertNotFoundException();
            }
            return ResponseFactory.ToResponse(response);
        }
    }
}
