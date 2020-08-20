using System;
using AutoFixture;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        [Test]
        public void CanMapACautionaryAlertDomainToResponse()
        {
            var fixture = new Fixture();
            var domain = fixture.Create<CautionaryAlert>();
            var response = domain.ToResponse();

            response.Description.Should().Be(domain.Description);
            response.AlertCode.Should().Be(domain.AlertCode);
            response.ModifiedBy.Should().Be(domain.ModifiedBy);
            response.DateModified.Should().Be(DateTimeToDateString(domain.DateModified));
            response.EndDate.Should().Be(DateTimeToDateString(domain.EndDate.Value));
            response.StartDate.Should().Be(DateTimeToDateString(domain.StartDate.Value));
        }

        [Test]
        public void CanMapACautionaryAlertPersonToAResponse()
        {
            var fixture = new Fixture();
            var domain = fixture.Create<CautionaryAlertPerson>();
            var response = domain.ToResponse();

            response.Alerts.Should().BeEquivalentTo(domain.Alerts.ToResponse());
            response.ContactNumber.Should().Be(domain.ContactNumber);
            response.PersonNumber.Should().Be(domain.PersonNumber);
            response.TenancyAgreementReference.Should().Be(domain.TagRef);
        }

        private static string DateTimeToDateString(DateTime date)
        {
            return $"{date.Year}-{date.Month:00}-{date.Day:00}";
        }

        [Test]
        public void CanMapAPropertyCautionaryAlertToAResponse()
        {
            var fixture = new Fixture();
            var domain = fixture.Create<CautionaryAlertsProperty>();
            var response = domain.ToResponse();

            response.Alerts.Should().BeEquivalentTo(domain.Alerts.ToResponse());
            response.AddressNumber.Should().Be(domain.AddressNumber);
            response.PropertyReference.Should().Be(domain.PropertyReference);
            response.UPRN.Should().Be(domain.UPRN);
        }
    }
}
