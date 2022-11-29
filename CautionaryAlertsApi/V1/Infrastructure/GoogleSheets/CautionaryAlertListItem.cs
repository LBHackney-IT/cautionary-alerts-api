using CautionaryAlertsApi.V1.Boundary.Response;
using System;

namespace CautionaryAlertsApi.V1.Gateways
{
    public class CautionaryAlertListItem
    {
        public string Name { get; set; }
        public string DoorNumber { get; set; }
        public string Address { get; set; }
        public string Neighbourhood { get; set; }
        public string DateOfIncident { get; set; }
        public string NumberOfDaysOutstanding { get; set; }
        public string Code { get; set; }
        public string LetterSent { get; set; }
        public string OnCivica { get; set; }
        public string Outcome { get; set; }
        public string CautionOnSystem { get; set; }
        public string ActionOnAssure { get; set; }
        public string Lookup { get; set; }
        public string PropertyReference { get; set; }
        public string TenancyDates { get; set; }
        public string IncidentBeforeCurrentTenancyDate { get; set; }
        public string Reason { get; set; }
        public string AssureReference { get; set; }
    }
}
