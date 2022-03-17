using System;
using System.Linq;
using NUnit.Framework;
using CautionaryAlertsApi.V1.Gateways;
using FluentAssertions;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Resources;
using CautionaryAlertsApi.Tests.V1.Factories;
using CautionaryAlertsApi.Tests.V1.Helper;
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

        [SetUp]
        public void SetUp()
        {
            var clientFactory = new FakeHttpClientFactory(new TestSpreadsheetHandler("test_cautionary_data.csv").RequestHandler);
            var baseClientService = new BaseClientService.Initializer { HttpClientFactory = clientFactory };

            _sheetService = new SheetsService(baseClientService);
            _classUnderTest = new GoogleSheetGateway(_sheetService);
        }

        [Test]
        public void GetsCautionaryAlertListItemForPropertyReference()
        {
            // Arrange
            const string propertyReference = "00999998";

            // Act
            var result = _classUnderTest.GetPropertyAlerts(propertyReference).ToList();
            TestContext.Out.Write(
                JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
                Environment.NewLine);

            // Assert
            result.Count.Should().Be(2);
            result.Should().ContainSingle(alert => alert.Address == "Fake Place 4");
            result.Should().ContainSingle(alert => alert.Address == "Fake Place 5");
            result.Should().OnlyContain(alert => alert.PropertyReference == propertyReference);
        }

        [Test]
        public void GetsCautionaryAlertListItemForPersonId()
        {
            // Arrange
            const string personId = "566c45c2-1f0c-4ecf-8fbf-afe62d51c8ba";

            // Act
            var result = _classUnderTest.GetPersonAlerts(personId).ToList();

            TestContext.Out.Write(
                JsonConvert.SerializeObject(result, Formatting.Indented, new StringEnumConverter()) +
                Environment.NewLine);

            // Assert
            result.Should().ContainSingle(alert => alert.CautionOnSystem == "Caution Type 2" && alert.Outcome == "Caution Description 2");
            result.Should().ContainSingle(alert => alert.CautionOnSystem == "Caution Type 5" && alert.Outcome == "Caution Description 5");
        }
    }
}
