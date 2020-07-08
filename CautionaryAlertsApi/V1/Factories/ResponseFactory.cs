using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static CautionaryAlertResponse ToResponse(this CautionaryAlert domain)
        {
            return new CautionaryAlertResponse
            {
                Description = domain.Description,
                AlertCode = domain.AlertCode,
                DateModified = domain.DateModified.ToString("yyyy-MM-dd"),
                EndDate = domain.EndDate?.ToString("yyyy-MM-dd"),
                ModifiedBy = domain.ModifiedBy,
                StartDate = domain.StartDate.ToString("yyyy-MM-dd"),
            };
        }

        public static CautionaryAlertPersonResponse ToResponse(this CautionaryAlertPerson domain)
        {
            return new CautionaryAlertPersonResponse
            {
                ContactNumber = domain.ContactNumber,
                PersonNumber = domain.PersonNumber,
                TagRef = domain.TagRef,
                Alerts = domain.Alerts.ToResponse()
            };
        }

        public static List<CautionaryAlertResponse> ToResponse(this IEnumerable<CautionaryAlert> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }

        public static CautionaryAlertsPropertyResponse ToResponse(this CautionaryAlertsProperty domain)
        {
            return new CautionaryAlertsPropertyResponse
            {
                AddressNumber = domain.AddressNumber,
                PropertyReference = domain.PropertyReference,
                Alerts = domain.Alerts.ToResponse()
            };
        }
    }
}
