using FluentAssertions;
using NUnit.Framework;
using CautionaryAlertsApi.V1.UseCase;

namespace CautionaryAlertsApi.Tests.V1.UseCase
{
    public class ValidatePropertyReferenceTests
    {
        [TestCase("100002105292")]
        [TestCase("100002664")]
        [TestCase("100007761")]
        [TestCase("00027048")]
        [TestCase("00028048")]
        [TestCase("00075715")]
        [TestCase("X1111111")]
        [TestCase("NS1111111")]
        [TestCase("x1111111")]
        [TestCase("ns1111111")]
        public void ValidPropertyReferencesReturnTrue(string postcode)
        {
            var validator = new ValidatePropertyReference();
            validator.Execute(postcode).Should().BeTrue();
        }

        [TestCase("1")]
        [TestCase("blahblah")]
        [TestCase("BLAHBLAH")]
        [TestCase("1100002105292")]
        [TestCase("111111")]
        [TestCase("xxx1111111")]
        [TestCase("X111111")]
        [TestCase(" ")]
        public void ValidPropertyReferencesReturnsFalse(string postcode)
        {
            var validator = new ValidatePropertyReference();
            validator.Execute(postcode).Should().BeFalse();
        }
    }
}
