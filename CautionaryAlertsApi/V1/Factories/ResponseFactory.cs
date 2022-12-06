using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;

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
                StartDate = domain.StartDate?.ToString("yyyy-MM-dd")
            };
        }

        public static CautionaryAlertPersonResponse ToResponse(this CautionaryAlertPerson domain)
        {
            return new CautionaryAlertPersonResponse
            {
                ContactNumber = domain.ContactNumber,
                PersonNumber = domain.PersonNumber,
                TenancyAgreementReference = domain.TagRef,
                Alerts = domain.Alerts.ToResponse()
            };
        }

        public static List<CautionaryAlertResponse> ToResponse(this IEnumerable<CautionaryAlert> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }

        public static List<CautionaryAlertPersonResponse> ToResponse(this IEnumerable<CautionaryAlertPerson> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }

        public static CautionaryAlertsPropertyResponse ToResponse(this CautionaryAlertsProperty domain)
        {
            return new CautionaryAlertsPropertyResponse
            {
                AddressNumber = domain.AddressNumber,
                PropertyReference = domain.PropertyReference,
                UPRN = domain.UPRN,
                Alerts = domain.Alerts.ToResponse(),
                AssureReference = domain.AssureReference
            };
        }

        public static CautionaryAlertGoogleSheetResponse ToCautionaryAlertGoogleSheetResponse(this CautionaryAlertListItem domain)
        {
            return new CautionaryAlertGoogleSheetResponse
            {
                Code = domain.Code,
                Type = domain.CautionOnSystem,
                Description = domain.Outcome
            };
        }

        public static CautionaryAlertResponse ToResponse(this CautionaryAlertListItem domain)
        {
            return new CautionaryAlertResponse
            {
                DateModified = domain.DateOfIncident,
                ModifiedBy = "GoogleSheet",
                StartDate = domain.DateOfIncident,
                AlertCode = domain.Code,
                Description = domain.CautionOnSystem,
                Reason = domain.Reason
            };
        }

        public static CautionaryAlertListItem ToResponse(this PropertyAlertDomain domain)
        {
            return new CautionaryAlertListItem()
            {
                DoorNumber = domain.DoorNumber,
                Address = domain.Address,
                AssureReference = domain.AssureReference,
                Code = domain.Code,
                DateOfIncident = domain.DateOfIncident,
                CautionOnSystem = domain.CautionOnSystem,
                Name = domain.PersonName,
                PropertyReference = domain.PropertyReference,
                Reason = domain.Reason,
                Neighbourhood = domain.Neighbourhood
            };
        }
    }
}
