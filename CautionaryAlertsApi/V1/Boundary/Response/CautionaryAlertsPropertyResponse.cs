using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class CautionaryAlertsPropertyResponse
    {
        public string PropertyReference { get; set; }
        public string UPRN { get; set; }
        public string AddressNumber { get; set; }
        public List<CautionaryAlertResponse> Alerts { get; set; }
        public string AssureReference { get; set; }
    }
}
