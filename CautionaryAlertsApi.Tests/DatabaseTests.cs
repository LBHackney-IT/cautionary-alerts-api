using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using System;

namespace CautionaryAlertsApi.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private IDbContextTransaction _transaction;
        protected UhContext UhContext { get; private set; }

        [SetUp]
        public void RunBeforeAnyTests()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = new DbContextOptionsBuilder();

            var connectionString = ConnectionString.TestDatabase();

            builder.UseNpgsql(connectionString);
            UhContext = new UhContext(builder.Options);

            UhContext.Database.EnsureCreated();

            _transaction = UhContext.Database.BeginTransaction();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
