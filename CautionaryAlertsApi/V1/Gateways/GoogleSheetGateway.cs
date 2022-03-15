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
            var rowIndices = FindRowIndices(propertyReference, "N");

            if (rowIndices.Any())
            {
                var row = GetSingleRow(rowIndices.First());
                return new List<CautionaryAlertListItem> { row.ToModel() };
            }

            return new List<CautionaryAlertListItem>();
        }

        public IEnumerable<CautionaryAlertListItem> GetPersonAlerts(string personId)
        {
            var rowIndices = FindRowIndices(personId, "AH").Where(rowIndex => rowIndex != -1);
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

        private IEnumerable<string> GetSingleRow(int rowIndex)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, $"CURRENT LIST!A{rowIndex}:P{rowIndex}");
            var response = request.Execute();
            var data = response.Values.First()
                .Select(cd => cd.ToString());

            return data;
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
                    range.Values.FirstOrDefault()?
                        .Select(cell => cell.ToString()));
        }
    }
}
