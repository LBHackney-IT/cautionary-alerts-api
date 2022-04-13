using System;
using System.Collections.Generic;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class CautionaryAlertsMMHPersonResponse
    {
        public Guid PersonId { get; set; }
        public List<CautionaryAlertResponse> Alerts { get; set; }
    }

    public class ListMMHPersonsCautionaryAlerts
    {
        public List<CautionaryAlertPersonResponse> Contacts { get; set; }
    }
}
