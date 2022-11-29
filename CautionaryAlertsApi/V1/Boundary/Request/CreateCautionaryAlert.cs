using System;

namespace CautionaryAlertsApi.V1.Boundary.Request
{
    public class CreateCautionaryAlert
    {
        public string AssureReference { get; set; }
        public DateTime IncidentDate { get; set; }
        public string IncidentDescription { get; set; }
        public Alert Alert { get; set; }
        public AssetDetails AssetDetails { get; set; }
        public PersonDetails PersonDetails { get; set; }
    }
}
