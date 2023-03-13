using System.Data.Common;
using CautionaryAlertsApi.Tests.V1.Factories;
using CautionaryAlertsApi.Tests.V1.Helper;
using Hackney.Shared.CautionaryAlerts.Infrastructure;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Hackney.Core.Sns;
using System;
using CautionaryAlertsApi.V1.Domain;
using Hackney.Core.Testing.Sns;
using NUnit.Framework;
using System.Net.Http;
using Amazon.SimpleNotificationService;

namespace CautionaryAlertsApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbConnection _connection;
        public HttpClient Client { get; private set; }

        public ISnsFixture SnsFixture { get; private set; }
        //public IAmazonSimpleNotificationService SimpleNotificationService { get; private set; }



        public MockWebApplicationFactory(DbConnection connection)
        {
            _connection = connection;
            EnsureEnvVarConfigured("Sns_LocalMode", "true");
            EnsureEnvVarConfigured("Sns_LocalServiceUrl", "http://localhost:8000");

            Client = CreateClient();
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var dbBuilder = new DbContextOptionsBuilder();
                dbBuilder.UseNpgsql(_connection);
                var context = new UhContext(dbBuilder.Options);
                services.AddSingleton(context);

                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<UhContext>();
                dbContext.Database.EnsureCreated();
             
                services.ConfigureSns();
                services.ConfigureSnsFixture();

                SnsFixture = serviceProvider.GetRequiredService<ISnsFixture>();
                SnsFixture.CreateSnsTopic<CautionaryAlertSns>("cautionaryAlert.fifo", "CAUTIONARY_ALERTS_SNS_ARN");
            });

            builder.ConfigureTestServices(services =>
            {
                var clientFactory = new FakeHttpClientFactory(new TestSpreadsheetHandler("test_cautionary_data.csv").RequestHandler);
                var baseClientService = new BaseClientService.Initializer { HttpClientFactory = clientFactory };

                var sheetService = new SheetsService(baseClientService);

                services.RemoveAll<SheetsService>();
                services.AddScoped(provider => sheetService);
            });
            
        }
    }
}
