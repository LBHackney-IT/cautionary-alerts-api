using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class DiscretionAlertsPersonResponse
    {
        public string PersonId { get; set; }
        public List<DiscretionAlertResponse> Alerts { get; set; }
    }
}
