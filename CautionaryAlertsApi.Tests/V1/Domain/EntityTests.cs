using System;
using CautionaryAlertsApi.V1.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests.V1.Domain
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void EntitiesHaveAnId()
        {
            var entity = new CautionaryAlert();
            entity.Id.Should().BeGreaterOrEqualTo(0);
        }
    }
}
