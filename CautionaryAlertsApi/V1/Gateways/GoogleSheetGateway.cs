using System;
using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Factories;
using Google.Apis.Sheets.v4;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum;

namespace CautionaryAlertsApi.V1.Gateways
{
    public class GoogleSheetGateway : IGoogleSheetGateway
    {
        private static readonly string _spreadsheetId =
            Environment.GetEnvironmentVariable("SPREADSHEET_ID") ?? "testing";

        private readonly SheetsService _sheetsService;

        public GoogleSheetGateway(SheetsService sheetService)
        {
            _sheetsService = sheetService;
        }

        public IEnumerable<CautionaryAlertListItem> GetPropertyAlerts(string propertyReference)
        {
            var rowNumber = GetRowIndex(propertyReference);
            var row = GetRow(rowNumber);

            return new List<CautionaryAlertListItem> { row.ToModel() };
        }

        private int GetRowIndex(string propertyReference)
        {
            // Only look at the property references column
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, "CURRENT LIST!N1:N1000");
            request.MajorDimension = COLUMNS;
            var data = request.Execute();
            var pRefs = data.Values.First().Select(value => value.ToString()).ToList();

            var goodPRefs = pRefs
                .Select((value, index) => new { value, index })
                .Where(p => p.value.Length > 0 && p.value != "Not found" && p.value != "#REF!")
                .ToList();

            var badPRefs = pRefs
                .Select((value, index) => new { value, index })
                .Except(goodPRefs);
            Console.WriteLine(
                $"{badPRefs.Count()} rows contain invalid property references and were excluded from the result.");

            var matchedRowIndex = goodPRefs.First(r => r.value == propertyReference).index + 1;
            Console.WriteLine(
                $"Property reference {propertyReference} {(matchedRowIndex is -1 ? "not found." : $"found on row {matchedRowIndex + 1}.")}");

            return matchedRowIndex;
        }

        private IEnumerable<string> GetRow(int row)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, $"CURRENT LIST!A{row}:P{row}");
            var response = request.Execute();
            var data = response.Values.First()
                .Select(cd => cd.ToString());

            return data;
        }
    }
}
