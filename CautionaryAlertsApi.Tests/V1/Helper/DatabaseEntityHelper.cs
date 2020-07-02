using AutoFixture;
using CautionaryAlertsApi.V1.Infrastructure;

namespace CautionaryAlertsApi.Tests.V1.Helper
{
    public static class DatabaseEntityHelper
    {
        public static PersonAlert CreatePeopleAlert()
        {
            return new Fixture().Create<PersonAlert>();
        }
    }
}
