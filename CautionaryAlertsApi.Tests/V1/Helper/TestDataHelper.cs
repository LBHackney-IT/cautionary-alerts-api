using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CautionaryAlertsApi.Tests.V1.Helper
{
    public static class TestDataHelper
    {
        public static AddressLink AddAddressLinkToDb(UhContext context, Fixture fixture, string propertyReference = null, int? id = null)
        {
            var addressLink = fixture.Create<AddressLink>();
            addressLink.PropertyReference = propertyReference ?? addressLink.PropertyReference;
            addressLink.AddressNumber = id ?? addressLink.AddressNumber;
            addressLink.DateModified = addressLink.DateModified;
            context.Addresses.Add(addressLink);
            context.SaveChanges();
            return addressLink;
        }

        public static PropertyAlert AddAlertToDatabaseForProperty(UhContext context, Fixture fixture, int addressNumber, DateTime? endDate = null)
        {
            var alert = fixture.Build<PropertyAlert>()
                .With(x => x.AddressNumber, addressNumber)
                .With(x => x.EndDate, endDate)
                .Without(x => x.AddressLink)
                .Create();
            context.PropertyAlerts.Add(alert);
            context.SaveChanges();
            return alert;
        }
        public static AlertDescriptionLookup AddDescriptionToDatabase(UhContext context, Fixture fixture, string code)
        {
            var desc = fixture.Build<AlertDescriptionLookup>()
                .With(d => d.AlertCode, code).Create();

            context.AlertDescriptionLookups.Add(desc);
            context.SaveChanges();
            return desc;
        }

        public static async Task SavePropertyAlertsToDb(UhContext context, IEnumerable<PropertyAlertNew> results)
        {
            context.PropertyAlertsNew.AddRange(results);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static async Task SavePropertyAlertToDb(UhContext context, PropertyAlertNew alert)
        {
            context.PropertyAlertsNew.Add(alert);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public static int SetStringLength(int initialLength, int dbConstraint)
        {
            return initialLength < dbConstraint ? initialLength : dbConstraint;
        }
    }
}
