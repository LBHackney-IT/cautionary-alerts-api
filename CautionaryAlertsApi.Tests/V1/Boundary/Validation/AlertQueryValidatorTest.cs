using CautionaryAlertsApi.V1.Boundary.Request;
using CautionaryAlertsApi.V1.Boundary.Request.Validation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using System;

namespace CautionaryAlertsApi.Tests.Boundary.Validation
{
    [TestFixture]
    public class AlertQueryValidatorTest
    {
        private readonly AlertQueryValidator _classUnderTest;

        public AlertQueryValidatorTest()
        {
            _classUnderTest = new AlertQueryValidator();
        }

        [Test]
        public void AlertIdFailsIfEmpty()
        {
            var model = new AlertQueryObject { AlertId = Guid.Empty };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AlertId);

        }

        [Test]
        public void AlertIdNotFailsIfValid()
        {
            var model = new AlertQueryObject { AlertId = Guid.NewGuid() };
            var result = _classUnderTest.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AlertId);

        }
    }
}
