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

        private static string _dummyRows;
        private static string _dummyColumns;
        private static string _dummyPropRefColumn;
        private static string _dummyA3P3;
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        public GoogleSheetGatewayTests()
        {
            _dummyRows = ReadFile("cc_sample.json");
            _dummyColumns = ReadFile("cc_sample_columns.json");
            _dummyPropRefColumn = ReadFile("cc_sample_columns_prop-refs.json");
            _dummyA3P3 = ReadFile("cc_sample_A3P3.json");

            string ReadFile(string fileName)
            {
                var resourceName = _assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName, StringComparison.CurrentCulture));
                using var stream = _assembly.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream ?? throw new InvalidOperationException("JSON file missing"));

                return reader.ReadToEnd();
            }
        }
        [SetUp]
        public void SetUp()
        {
            static HttpResponseMessage RequestHandler(HttpRequestMessage request)
            {
                TestContext.Out.WriteLine(request.RequestUri.PathAndQuery);
                TestContext.Out.WriteLine(request.RequestUri.Query);

                if (request.RequestUri.Query.Contains("ROWS", StringComparison.CurrentCulture))
                    return new HttpResponseMessage { Content = new StringContent(_dummyRows), StatusCode = HttpStatusCode.OK };

                if (request.RequestUri.PathAndQuery.Contains("CURRENT%20LIST%21N1%3AN1000", StringComparison.CurrentCulture))
                    return new HttpResponseMessage { Content = new StringContent(_dummyPropRefColumn) };

                if (request.RequestUri.PathAndQuery.Contains("CURRENT%20LIST%21A3%3AP3", StringComparison.CurrentCulture))
                    return new HttpResponseMessage { Content = new StringContent(_dummyA3P3) };

                return new HttpResponseMessage
                { Content = new StringContent(_dummyColumns), StatusCode = HttpStatusCode.OK };
            }

            var clientFactory = new FakeHttpClientFactory(RequestHandler);
            var baseClientService = new BaseClientService.Initializer { HttpClientFactory = clientFactory };
            _sheetService = new SheetsService(baseClientService);
            _classUnderTest = new GoogleSheetGateway(_sheetService);
        }

        // [Test]
        // public void ListsAllCautionaryAlertListItems()
        // {
        //     // Act
        //     var result = _classUnderTest.ListPropertyAlerts();
        //     TestContext.Out.Write(JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
        //                   Environment.NewLine);
        //
        //     //Assert
        //     result.Count().Should().Be(2);
        // }

        [Test]
        public void GetsCautionaryAlertListItemForPropertyReference()
        {
            // Arrange
            const string propertyReference = "00999998";

            // Act
            var result = _classUnderTest.GetPropertyAlerts(propertyReference);
            TestContext.Out.Write(
                JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
                Environment.NewLine);

            // Assert
            result.First().PropertyReference.Should().Be(propertyReference);
        }
    }
}
