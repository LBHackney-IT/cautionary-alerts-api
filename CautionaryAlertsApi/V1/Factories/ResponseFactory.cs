using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Boundary.Response;
using CautionaryAlertsApi.V1.Domain;

namespace CautionaryAlertsApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Factory-object-mappings
        public static CautionaryAlertResponse ToResponse(this CautionaryAlert domain)
        {
            return new CautionaryAlertResponse();
        }

        public static List<CautionaryAlertResponse> ToResponse(this IEnumerable<CautionaryAlert> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
