namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class CautionaryAlertResponse
    {
        //TODO: add xml comments containing information that will be included in the auto generated swagger docs
        public string DateModified { get; set; }

        public string ModifiedBy { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string AlertCode { get; set; }

        public string Description { get; set; }
    }
}
