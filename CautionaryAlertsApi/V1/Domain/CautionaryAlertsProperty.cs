using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.V1.Domain
{
    public class CautionaryAlertsProperty
    {
        public string PropertyReference { get; set; }
        public string UPRN { get; set; }
        public string AddressNumber { get; set; }
        public List<CautionaryAlert> Alerts { get; set; }
    }
}
