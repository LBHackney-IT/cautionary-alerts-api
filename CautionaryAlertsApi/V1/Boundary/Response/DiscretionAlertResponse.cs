namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class DiscretionAlertResponse
    {
        /// <summary>
        /// A unique code for alert
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A type of discretion to keep when dealing with a person
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Description of the nature of the discretion alert
        /// </summary>
        public string Description { get; set; }
    }
}
