namespace CautionaryAlertsApi.V1.Domain
{
    public class PropertyAlertDomain
    {
        public int Id { get; set; }
        public string DoorNumber { get; set; }
        public string Address { get; set; }
        public string Neighbourhood { get; set; }
        public string DateOfIncident { get; set; }
        public string Code { get; set; }
        public string CautionOnSystem { get; set; }
        public string PropertyReference { get; set; }
        public string UPRN { get; set; }
        public string PersonName { get; set; }
        public string MMHID { get; set; }
        public string Reason { get; set; }
        public string AssureReference { get; set; }
    }
}
