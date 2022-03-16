namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class CautionaryAlertGoogleSheetResponse
    {
        /// <summary>
        /// A unique code for alert
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A type of cautionary to keep when dealing with a person
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Description of the nature of the cautionary alert
        /// </summary>
        public string Description { get; set; }
    }
}
