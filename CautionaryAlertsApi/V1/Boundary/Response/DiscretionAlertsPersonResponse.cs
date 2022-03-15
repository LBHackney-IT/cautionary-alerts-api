using System.Collections.Generic;

namespace CautionaryAlertsApi.V1.Boundary.Response
{
    public class DiscretionAlertsPersonResponse
    {
        /// <summary>
        /// A unique MMH identifier (GUID) of a person
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// A list of discretion alerts for a person with <see cref="PersonId"/>
        /// </summary>
        public List<DiscretionAlertResponse> Alerts { get; set; }
    }
}
