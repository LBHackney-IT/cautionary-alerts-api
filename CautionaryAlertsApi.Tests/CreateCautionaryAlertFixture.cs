using AutoFixture;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using Hackney.Shared.CautionaryAlerts.Domain;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using System;

namespace CautionaryAlertsApi.Tests
{
    public static class CreateCautionaryAlertFixture
    {
        public static CreateCautionaryAlert GenerateValidCreateCautionaryAlertFixture(string defaultString, Fixture fixture, string addressString)
        {
            var alert = fixture.Build<Alert>()
                .With(x => x.Code, defaultString[..CreateCautionaryAlertConstants.ALERTCODELENGTH])
                .With(x => x.Description, defaultString[..CreateCautionaryAlertConstants.ALERTDESCRIPTION])
                .Create();

            var assetDetails = fixture.Build<AssetDetails>()
                .With(x => x.FullAddress, addressString[..CreateCautionaryAlertConstants.FULLADDRESSLENGTH])
                .With(x => x.PropertyReference, defaultString[..CreateCautionaryAlertConstants.PROPERTYREFERENCELENGTH])
                .With(x => x.UPRN, defaultString[..CreateCautionaryAlertConstants.UPRNLENGTH])
                .Create();

            var personDetails = fixture.Build<PersonDetails>()
                .With(x => x.Name, defaultString[..CreateCautionaryAlertConstants.PERSONNAMELENGTH])
                .Create();

            var cautionaryAlert = fixture.Build<CreateCautionaryAlert>()
                .With(x => x.Alert, alert)
                .With(x => x.PersonDetails, personDetails)
                .With(x => x.AssetDetails, assetDetails)
                .With(x => x.IncidentDescription, defaultString[..CreateCautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH])
                .With(x => x.IncidentDate, fixture.Create<DateTime>().AddDays(-1))
                .With(x => x.AssureReference, defaultString[..CreateCautionaryAlertConstants.ASSUREREFERENCELENGTH])
                .Create();

            return cautionaryAlert;
        }

        public static CreateCautionaryAlert GenerateValidCreateCautionaryAlertWithoutAssetDetailsFixture(string defaultString, Fixture fixture)
        {
            var alert = fixture.Build<Alert>()
                .With(x => x.Code, defaultString[..CreateCautionaryAlertConstants.ALERTCODELENGTH])
                .With(x => x.Description, defaultString[..CreateCautionaryAlertConstants.ALERTDESCRIPTION])
                .Create();


            var personDetails = fixture.Build<PersonDetails>()
                .With(x => x.Name, defaultString[..CreateCautionaryAlertConstants.PERSONNAMELENGTH])
                .Create();

            var cautionaryAlert = fixture.Build<CreateCautionaryAlert>()
                .With(x => x.Alert, alert)
                .With(x => x.PersonDetails, personDetails)
                .With(x => x.IncidentDescription, defaultString[..CreateCautionaryAlertConstants.INCIDENTDESCRIPTIONLENGTH])
                .With(x => x.IncidentDate, fixture.Create<DateTime>().AddDays(-1))
                .With(x => x.AssureReference, defaultString[..CreateCautionaryAlertConstants.ASSUREREFERENCELENGTH])
                .Without(x => x.AssetDetails)
                .Create();

            return cautionaryAlert;
        }

        public static EndCautionaryAlert GenerateValidEndCautionaryAlertFixture(CreateCautionaryAlert existingCautionaryAlert, Fixture fixture)
        {
            
            var endCautionaryAlert = fixture.Build<EndCautionaryAlert>()
                .With(x => x.Alert, existingCautionaryAlert.Alert)
                .With(x => x.PersonDetails, existingCautionaryAlert.PersonDetails)
                .With(x => x.AssetDetails, existingCautionaryAlert.AssetDetails)
                .With(x => x.IncidentDescription, existingCautionaryAlert.IncidentDescription)
                .With(x => x.IncidentDate, existingCautionaryAlert.IncidentDate)
                .With(x => x.AssureReference, existingCautionaryAlert.AssureReference)
                .With(x=> x.IsActive, false)
                .With(x=> x.AlertId, existingCautionaryAlert.AlertId)
                .Create();

            return endCautionaryAlert;
        }
    }
}
