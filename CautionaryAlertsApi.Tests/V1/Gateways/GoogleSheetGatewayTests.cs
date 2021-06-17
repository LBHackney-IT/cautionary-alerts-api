using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using NUnit.Framework;
using CautionaryAlertsApi.V1.Gateways;
using FluentAssertions;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Resources;
using CautionaryAlertsApi.Tests.V1.Factories;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[assembly: NeutralResourcesLanguage("en")]

namespace CautionaryAlertsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class GoogleSheetGatewayTests
    {
        private SheetsService _sheetService;
        private GoogleSheetGateway _classUnderTest;
        private static readonly string _sampleSpreadsheetJson = GetSampleSpreadsheetJson();

        private static string GetSampleSpreadsheetJson()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("sample_sheet.json", StringComparison.CurrentCulture));

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException("sample JSON missing"));
            return reader.ReadToEnd();
        }

        [SetUp]
        public void SetUp()
        {
            _sheetService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientFactory = new FakeHttpClientFactory(request => new HttpResponseMessage
                {
                    Content = new StringContent(_sampleSpreadsheetJson),
                    StatusCode = HttpStatusCode.OK
                })
            });

            _classUnderTest = new GoogleSheetGateway(_sheetService);
        }

        [Test]
        public void GetsEntitiesSuccessfully()
        {
            // Act
            var result = _classUnderTest.GetRows();
            TestContext.Out.Write(JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
                          Environment.NewLine);

            //Assert
            result.Count().Should().Be(8);
        }
    }
}
