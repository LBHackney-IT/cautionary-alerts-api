using System;
using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Factories;
using Google.Apis.Sheets.v4;
using static CautionaryAlertsApi.V1.Helpers.GoogleSheetHelpers;
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
            if (rowNumber == -1) return new List<CautionaryAlertListItem>();

            var row = GetRow(rowNumber);

            return new List<CautionaryAlertListItem> { row.ToModel() };
        }

        private int GetRowIndex(string propertyReference)
        {
            // Only look at the property references column
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, "CURRENT LIST!N1:N1000");
            request.MajorDimension = COLUMNS;
            var sheetData = request.Execute();

            var propertyRefs = sheetData
                .GetAllPropertyRefs()
                .ExcludeInvalidPropertyRefs();

            if (propertyRefs is null) return -1;

            var firstprop = propertyRefs.FirstOrDefault(r => r.Value == propertyReference);

            if (firstprop is null) return -1;

            var firstMatchingRow = firstprop.Index + 1;

            Console.WriteLine(
                $"Property reference {propertyReference} {(firstMatchingRow is -1 ? "not found." : $"found on row {firstMatchingRow + 1}.")}");

            return firstMatchingRow;
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
