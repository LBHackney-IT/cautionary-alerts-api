namespace CautionaryAlertsApi.V1.Domain
{
    public class CautionaryAlertPerson
    {
        public string TagRef { get; set; }
        public string PersonNumber { get; set; }
        public string ContactNumber { get; set; }
        public CautionaryAlert Alert { get; set; }
    }
}
