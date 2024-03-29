using System;
using System.Linq;
using System.Net.Http;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NUnit.Framework;

namespace CautionaryAlertsApi.Tests
{
    public class IntegrationTests<TStartup> where TStartup : class
    {
        protected HttpClient Client { get; private set; }
        protected UhContext UhContext { get; private set; }

        public MockWebApplicationFactory<TStartup> Factory;
        private NpgsqlConnection _connection;
        private DbContextOptionsBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            if (Environment.GetEnvironmentVariable("MANAGE_CAUTIONARY_ALERT_ALLOWED_GROUPS") == null)
                Environment.SetEnvironmentVariable("MANAGE_CAUTIONARY_ALERT_ALLOWED_GROUPS", "e2e-testing");

            _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
            _connection.Open();
            var npgsqlCommand = _connection.CreateCommand();
            npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
            npgsqlCommand.ExecuteNonQuery();

            _builder = new DbContextOptionsBuilder();
            _builder.UseNpgsql(_connection);

        }

        [SetUp]
        public void BaseSetup()
        {
            Factory = new MockWebApplicationFactory<TStartup>(_connection);

            Client = Factory.CreateClient();

            UhContext = new UhContext(_builder.Options);

            UhContext.Database.EnsureCreated();
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            Factory.Dispose();
            ClearDatabaseData(UhContext);
        }

        public void ClearDatabaseData(UhContext context)
        {
            context.PeopleAlerts.RemoveRange(context.PeopleAlerts);
            context.AlertDescriptionLookups.RemoveRange(context.AlertDescriptionLookups);
            context.ContactLinks.RemoveRange(context.ContactLinks);
            context.Addresses.RemoveRange(context.Addresses);
            context.PropertyAlerts.RemoveRange(context.PropertyAlerts);
            context.PropertyAlertsNew.RemoveRange(context.PropertyAlertsNew);
            context.SaveChanges();
        }
    }
}
