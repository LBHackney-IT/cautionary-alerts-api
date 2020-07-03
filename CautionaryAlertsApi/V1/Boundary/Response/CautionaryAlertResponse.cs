namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class CautionaryAlertResponse
    {
        /// <remarks>date format</remarks>
        /// <summary>
        ///Date the cautionary alert was last modified
        /// </summary>
        /// <example>2019-06-30</example>
        public string DateModified { get; set; }

        /// <summary>
        /// The username for the officer who last modified the cautionary alert
        /// </summary>
        /// <example>LSAMBA</example>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// The date that the cautionary alert starts
        /// </summary>
        /// <example>2019-03-30</example>
        public string StartDate { get; set; }
        /// <summary>
        /// The date that the cautionary alert will end. null if not applicable/ unknown.
        /// </summary>
        /// <example>2020-01-13</example>
        public string EndDate { get; set; }
        /// <summary>
        /// Unique code for the cautionary alert
        /// </summary>
        /// <example>VA</example>
        public string AlertCode { get; set; }
        /// <summary>
        /// Description of the nature of the cautionary alert
        /// </summary>
        /// <example>Verbal abuse or threat of</example>
        public string Description { get; set; }
    }
}
