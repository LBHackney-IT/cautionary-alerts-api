using System.Data.Common;
using CautionaryAlertsApi.Tests.V1.Factories;
using CautionaryAlertsApi.Tests.V1.Helper;
using CautionaryAlertsApi.V1.Infrastructure;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CautionaryAlertsApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbConnection _connection;

        public MockWebApplicationFactory(DbConnection connection)
        {
            _connection = connection;
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

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
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
