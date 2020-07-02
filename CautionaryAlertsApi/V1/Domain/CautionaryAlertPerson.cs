using System.Collections.Generic;

namespace CautionaryAlertsApi.V1.Domain
{
    public class CautionaryAlertPerson
    {
        public string TagRef { get; set; }
        public string PersonNumber { get; set; }
        public string ContactNumber { get; set; }
        public List<CautionaryAlert> Alerts { get; set; }
    }
}
