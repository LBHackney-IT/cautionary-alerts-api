using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using System;

namespace CautionaryAlertsApi.Tests.V1.Boundary.Validation
{
    [TestFixture]
    public class EndCautionaryAlertValidatorTests
    {
        private readonly EndCautionaryAlertValidator _classUnderTest;

        public EndCautionaryAlertValidatorTests()
        {
            _classUnderTest = new EndCautionaryAlertValidator();
        }

        [Test]
        public void EndDatePassesIfDateInPast()
        {
            var model = new EndCautionaryAlertRequest() { EndDate = DateTime.UtcNow.AddYears(-1) };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
        }

        [Test]
        public void EndDateFailsIfDateInFuture()
        {
            var model = new EndCautionaryAlertRequest() { EndDate = DateTime.UtcNow.AddYears(+1) };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.EndDate);
        }
    }
}
