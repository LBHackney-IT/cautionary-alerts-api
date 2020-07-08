using System;
using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Boundary.Response;
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
            return new List<CautionaryAlertPerson>();
        }

        public CautionaryAlertsProperty GetCautionaryAlertsForAProperty(string propertyReference)
        {
            var addressLink = _uhContext.Addresses.
                Where(x => x.PropertyReference == propertyReference).FirstOrDefault();

            if (addressLink == null)
            {
                return null;
            }

            var propertyAlerts = GetPropertyAlerts(addressLink)
                    .Select(GetDescriptionOfAlert)
                    .ToList();

            return new CautionaryAlertsProperty
            {
                AddressNumber = addressLink.AddressNumber.ToString(),
                PropertyReference = propertyReference,
                Alerts = propertyAlerts
            };
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
