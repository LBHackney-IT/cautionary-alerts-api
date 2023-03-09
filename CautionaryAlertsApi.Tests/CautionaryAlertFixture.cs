using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using System;
using System.Runtime.CompilerServices;

namespace CautionaryAlertsApi.Tests
{
    public static class CautionaryAlertFixture
    {
        public static CreateCautionaryAlert GenerateValidCreateCautionaryAlertFixture(string defaultString, Fixture fixture, string addressString)
        {
            var alert = fixture.Build<Alert>()
                .With(x => x.Code, defaultString[..CautionaryAlertConstants.ALERTCODELENGTH])
                .With(x => x.Description, defaultString[..CautionaryAlertConstants.ALERTDESCRIPTION])
                .Create();

            var assetDetails = fixture.Build<AssetDetails>()
                .With(x => x.FullAddress, addressString[..CautionaryAlertConstants.FULLADDRESSLENGTH])
                .With(x => x.PropertyReference, defaultString[..CautionaryAlertConstants.PROPERTYREFERENCELENGTH])
                .With(x => x.UPRN, defaultString[..CautionaryAlertConstants.UPRNLENGTH])
                .Create();

            var personDetails = fixture.Build<PersonDetails>()
                .With(x => x.Name, defaultString[..CautionaryAlertConstants.PERSONNAMELENGTH])
                .Create();

            var cautionaryAlert = fixture.Build<CreateCautionaryAlert>()
                .With(x => x.Alert, alert)
                .With(x => x.PersonDetails, personDetails)
                .With(x => x.AssetDetails, assetDetails)
                .With(x => x.IncidentDescription, defaultString[..CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH])
                .With(x => x.IncidentDate, fixture.Create<DateTime>().AddDays(-1))
                .With(x => x.AssureReference, defaultString[..CautionaryAlertConstants.ASSUREREFERENCELENGTH])
                .Create();

            return cautionaryAlert;
        }

        public static CreateCautionaryAlert GenerateValidCreateCautionaryAlertWithoutAssetDetailsFixture(string defaultString, Fixture fixture)
        {
            var alert = fixture.Build<Alert>()
                .With(x => x.Code, defaultString[..CautionaryAlertConstants.ALERTCODELENGTH])
                .With(x => x.Description, defaultString[..CautionaryAlertConstants.ALERTDESCRIPTION])
                .Create();


            var personDetails = fixture.Build<PersonDetails>()
                .With(x => x.Name, defaultString[..CautionaryAlertConstants.PERSONNAMELENGTH])
                .Create();

            var cautionaryAlert = fixture.Build<CreateCautionaryAlert>()
                .With(x => x.Alert, alert)
                .With(x => x.PersonDetails, personDetails)
                .With(x => x.IncidentDescription, defaultString[..CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH])
                .With(x => x.IncidentDate, fixture.Create<DateTime>().AddDays(-1))
                .With(x => x.AssureReference, defaultString[..CautionaryAlertConstants.ASSUREREFERENCELENGTH])
                .Without(x => x.AssetDetails)
                .Create();

            return cautionaryAlert;
        }

        public static EndCautionaryAlert GenerateValidEndCautionaryAlertFixture(Guid personId, Guid alertId, string defaultString, string addressString, Fixture fixture)
        {

            var alert = fixture.Build<Alert>()
                .With(x => x.Code, defaultString[..CautionaryAlertConstants.ALERTCODELENGTH])
                .With(x => x.Description, defaultString[..CautionaryAlertConstants.ALERTDESCRIPTION])
                .Create();

            var assetDetails = fixture.Build<AssetDetails>()
                .With(x => x.FullAddress, addressString[..CautionaryAlertConstants.FULLADDRESSLENGTH])
                .With(x => x.PropertyReference, defaultString[..CautionaryAlertConstants.PROPERTYREFERENCELENGTH])
                .With(x => x.UPRN, defaultString[..CautionaryAlertConstants.UPRNLENGTH])
                .Create();

            var personDetails = fixture.Build<PersonDetails>()
                .With(x => x.Name, defaultString[..CautionaryAlertConstants.PERSONNAMELENGTH])
                .With(x=> x.Id, personId)
                .Create();

            var cautionaryAlert = fixture.Build<EndCautionaryAlert>()
                .With(x => x.Alert, alert)
                .With(x => x.PersonDetails, personDetails)
                .With(x => x.AssetDetails, assetDetails)
                .With(x => x.IncidentDescription, defaultString[..CautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH])
                .With(x => x.IncidentDate, fixture.Create<DateTime>().AddDays(-1))
                .With(x => x.AssureReference, defaultString[..CautionaryAlertConstants.ASSUREREFERENCELENGTH])
                .With(x=> x.AlertId, alertId)
                .With(x=> x.IsActive, true)
                .Create();

            return cautionaryAlert;
        }
    }
}
