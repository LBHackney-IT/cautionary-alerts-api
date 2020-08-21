using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CautionaryAlertsApi.V1.Gateways
{
    public class UhGateway : IUhGateway
    {
        private readonly UhContext _uhContext;

        public UhGateway(UhContext uhContext)
        {
            _uhContext = uhContext;
        }

        public List<CautionaryAlertPerson> GetCautionaryAlertsForAPerson(string tagRef, string personNumber)
        {
            var links = _uhContext.ContactLinks
                .Where(c => c.Key == tagRef)
                .Where(c => c.PersonNumber == personNumber)
                .ToList();

            return links.Select(link =>
            {
                var personsAlerts = GetPersonAlerts(link)
                    .Select(GetDescriptionAndMapToDomain)
                    .ToList();

                return new CautionaryAlertPerson
                {
                    ContactNumber = link.ContactNumber.ToString(),
                    PersonNumber = link.PersonNumber,
                    TagRef = link.Key,
                    Alerts = personsAlerts
                };
            }).ToList();
        }

        private List<PersonAlert> GetPersonAlerts(ContactLink link)
        {
            return _uhContext.PeopleAlerts
                .Where(a => a.ContactNumber == link.ContactNumber).ToList();
        }
        private CautionaryAlert GetDescriptionAndMapToDomain(PersonAlert alert)
        {
            var description = _uhContext.AlertDescriptionLookups
                .OrderByDescending(a => a.DateModified)
                .FirstOrDefault(a => a.AlertCode == alert.AlertCode);
            return alert.ToDomain(description?.Description);
        }


        public CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference)
        {
            var addressLink = _uhContext.Addresses.FirstOrDefault(x => x.PropertyReference == propertyReference);

            if (addressLink == null)
            {
                return null;
            }
            else
            {
                var propertyAlerts = GetPropertyAlerts(addressLink)
                 .Select(GetDescriptionOfAlert)
                 .ToList();

                return new CautionaryAlertsProperty
                {
                    AddressNumber = addressLink.AddressNumber.ToString(),
                    PropertyReference = propertyReference,
                    UPRN = addressLink.UPRN,
                    Alerts = propertyAlerts
                };
            }
        }

        private List<PropertyAlert> GetPropertyAlerts(AddressLink addressLink)
        {
            return _uhContext.PropertyAlerts
              .Include(x => x.AddressLink)
              .Where(x => x.AddressNumber == addressLink.AddressNumber).ToList();
        }
        private CautionaryAlert GetDescriptionOfAlert(PropertyAlert alert)
        {
            var description = _uhContext.AlertDescriptionLookups
                .OrderByDescending(a => a.DateModified)
                .FirstOrDefault(a => a.AlertCode == alert.AlertCode);
            return alert.ToDomain(description?.Description);
        }
    }
}
