using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System;

namespace CautionaryAlertsApi.V1.Boundary.Request
{
    public class AlertQueryObject
    {
        [FromRoute(Name = "alertId")]
        public Guid AlertId { get; set; }
    }
}
