using System;
using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Factories;
using CautionaryAlertsApi.V1.Infrastructure.GoogleSheets;
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
            return FindAlerts(propertyReference, "N");
        }

        public IEnumerable<CautionaryAlertListItem> GetPersonAlerts(string personId)
        {
            return FindAlerts(personId, "AH");
        }

        private IEnumerable<CautionaryAlertListItem> FindAlerts(string query, string column)
        {
            var rowIndices = FindRowIndices(query, column);
            var matchingRows = GetRows(rowIndices);

            return matchingRows
                .Select(row => row.ToModel())
                .ToList();
        }

        private IList<int> FindRowIndices(string value, string column)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, $"CURRENT LIST!{column}1:{column}1000");
            request.MajorDimension = COLUMNS;

            var sheetData = request.Execute();
            var cellValues = sheetData
                .GetCellValues()
                .ExcludeInvalidCellValues();

            var matchingRowIndices = cellValues?
                .Where(cell => cell.Value == value)
                .Select(cell => cell.Index + 1)
                .ToList();

            return (matchingRowIndices != null && matchingRowIndices.Any())
                ? matchingRowIndices
                : new List<int>();
        }

        private IEnumerable<IEnumerable<string>> GetRows(IEnumerable<int> rowIndices)
        {
            var request = _sheetsService.Spreadsheets.Values.BatchGet(_spreadsheetId);

            request.Ranges = rowIndices
                .Select(rowIndex => $"CURRENT LIST!A{rowIndex}:P{rowIndex}")
                .ToList();

            var response = request.Execute();

            return response.ValueRanges
                .Select(range =>
                    range.Values
                        .FirstOrDefault()?
                        .Select(cell => cell.ToString()));
        }
    }
}
