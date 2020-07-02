using System.Collections.Generic;

namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class CautionaryAlertPersonResponse
    {
        public string TagRef { get; set; }
        public string PersonNumber { get; set; }
        public string ContactNumber { get; set; }
        public List<CautionaryAlertResponse> Alerts { get; set; }
    }
}
