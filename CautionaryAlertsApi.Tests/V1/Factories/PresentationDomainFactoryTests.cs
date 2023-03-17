using System;
using AutoFixture;
using CautionaryAlertsApi.V1.Factories;
using FluentAssertions;
using Hackney.Shared.CautionaryAlerts.Boundary.Request;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Factories
{
    [TestFixture]
    public class PresentationDomainFactoryTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void PresentationDomainFactoryCorrectlyMapsAlertQueryObjectToEndCautionaryAlert()
        {
            // arrange
            var presentationObj = _fixture.Create<AlertQueryObject>();

            // act
            var mappedDomainResult = presentationObj.ToDomain();

            // assert
            mappedDomainResult.AlertId.Should().Be(presentationObj.AlertId);
        }

        [Test]
        public void PresentationDomainFactoryThrowsArgumentNullExceptionWhenPresentationObjectIsNull()
        {
            // arrange
            var presentationObj = null as AlertQueryObject;

            // act
            Action mappingCall = () => presentationObj.ToDomain();

            // assert
            mappingCall.Should().Throw<ArgumentNullException>().WithMessage("*presentationObj*");
        }
    }
}
