using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Factories;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CautionaryAlert = Hackney.Shared.CautionaryAlerts.Domain.CautionaryAlert;
using Hackney.Shared.CautionaryAlerts.Infrastructure.GoogleSheets;
using PropertyAlert = Hackney.Shared.CautionaryAlerts.Infrastructure.PropertyAlert;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.Tests.V1.Infrastructure;
using Npgsql;
using CautionaryAlertsApi.Tests;
using CautionaryAlertsApi.V1.Boundary.Request;
using AlertQueryObject = CautionaryAlertsApi.V1.Boundary.Request.AlertQueryObject;

namespace CautionaryAlertsApi.V1.Gateways
{
    public class UhGateway : IUhGateway
    {
        private readonly UhContext _uhContext;
        private readonly ILogger<UhGateway> _logger;
        public UhGateway(UhContext uhContext, ILogger<UhGateway> logger)
        {
            _uhContext = uhContext;
            _logger = logger;
        }

        public List<CautionaryAlertPerson> GetCautionaryAlertsForPeople(string tagRef, string personNumber)
        {
            var links = _uhContext.ContactLinks
                .Where(c => c.Key == tagRef)
                .Where(c => string.IsNullOrEmpty(personNumber) || c.PersonNumber == personNumber)
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

                var assureReference = GetAssureReference(propertyReference);

                return new CautionaryAlertsProperty
                {
                    AddressNumber = addressLink.AddressNumber.ToString(),
                    PropertyReference = propertyReference,
                    UPRN = addressLink.UPRN,
                    Alerts = propertyAlerts,
                    AssureReference = assureReference
                };
            }
        }

        private string GetAssureReference(string propertyReference)
        {
            return _uhContext.PropertyAlertsNew
                .Where(x => x.PropertyReference == propertyReference)
                .Select(x => x.AssureReference)
                .FirstOrDefault();
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

        public async Task<IEnumerable<CautionaryAlertListItem>> GetPropertyAlertsNew(string propertyReference)
        {
            var alerts = await _uhContext.PropertyAlertsNew
                .Where(x => x.PropertyReference == propertyReference)
                .ToListAsync().ConfigureAwait(false);

            return alerts.Select(x => x.ToDomain());
        }

        public async Task<IEnumerable<CautionaryAlertListItem>> GetCautionaryAlertsByMMHPersonId(Guid personId)
        {
            var alerts = await _uhContext.PropertyAlertsNew
                .Where(x => x.MMHID == personId.ToString())
                .Where(x => x.IsActive == true)
                .ToListAsync().ConfigureAwait(false);

            return alerts.Select(x => x.ToDomain());
        }

        public PropertyAlertDomain GetCautionaryAlertByAlertId(AlertQueryObject query)
        {
            var alerts = _uhContext.PropertyAlertsNew.AsNoTracking()
                        .Where(x => x.AlertId == query.AlertId.ToString());

            var cautionaryAlert = alerts.Select(x => x.ToPropertyAlertDomain());

            //We should never expect the count to by more than one as AlertId should be unique but adding this condition as we only return the first alert. 
            var count = cautionaryAlert.Count();
            if (count > 1)
                throw new MoreThanOneAlertException(count);


            return cautionaryAlert.FirstOrDefault();
        }


        [LogCall]
        public async Task<PropertyAlertDomain> PostNewCautionaryAlert(CreateCautionaryAlert cautionaryAlert)
        {
            _logger.LogDebug($"Calling Postgress.SaveAsync");
            var alertId = Guid.NewGuid().ToString();
            var alertDbEntity = cautionaryAlert.ToDatabase(isActive: true, alertId);

            _uhContext.PropertyAlertsNew.Add(alertDbEntity);
            await _uhContext.SaveChangesAsync().ConfigureAwait(false);

            return alertDbEntity.ToPropertyAlertDomain();
        }

        [LogCall]
        public async Task<PropertyAlertDomain> EndCautionaryAlert(AlertQueryObject queryData, EndCautionaryAlertRequest endCautionaryAlertRequest)
        {
            var alertToBeUpdated = await _uhContext
                .PropertyAlertsNew
                .FirstOrDefaultAsync(x => x.AlertId == queryData.AlertId.ToString())
                .ConfigureAwait(false);

            if (alertToBeUpdated is null)
                return null;

            alertToBeUpdated.IsActive = false;
            alertToBeUpdated.EndDate = endCautionaryAlertRequest.EndDate;

            _logger.LogDebug($"Calling Postgress.SaveAsync");

            await _uhContext.SaveChangesAsync().ConfigureAwait(false);

            return alertToBeUpdated.ToPropertyAlertDomain();
        }
    }
}
