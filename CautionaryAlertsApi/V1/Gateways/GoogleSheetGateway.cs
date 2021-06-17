using System;
using System.Collections.Generic;
using System.Linq;
using CautionaryAlertsApi.V1.Domain;
using CautionaryAlertsApi.V1.Factories;
using static CautionaryAlertsApi.V1.Helpers.GoogleSheetHelpers;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace CautionaryAlertsApi.V1.Gateways
{
    public class GoogleSheetGateway : IGoogleSheetGateway
    {
        private const string SpreadsheetId = "some random ID";
        private readonly SheetsService _sheetsService;

        public GoogleSheetGateway(SheetsService sheetService)
        {
            _sheetsService = sheetService;
        }

        public IEnumerable<ExampleModel> GetRows()
        {
            var request = _sheetsService.Spreadsheets.Get(SpreadsheetId);
            request.IncludeGridData = true;

            var rowData = request.Execute()
                .Sheets.First()
                .Data.First()
                .RowData;

            var rows = rowData.Select(
                rd => rd.Values.Where(
                    cd => cd.FormattedValue != null).Select(
                    cd => cd.FormattedValue.ToString()
                ).ToList()
            ).ToList();

            var headers = rows.First();
            if (!IsValid<ExampleModel>(headers))
                throw new Exception("Spreadsheet schema does not match the provided type!");

            rows.RemoveAt(0);

            var highlightedFlags = GetHighlightedFlags(rowData);
            var pRows = ParseHighlighting(rows, highlightedFlags);
            PrintSpreadSheet(pRows);

            var entities = pRows.Select(
                row => row
                    .ToList()
                    .ToDomain()
            );

            return entities;
        }

        private static IEnumerable<IEnumerable<string>> ParseHighlighting(
            IEnumerable<IEnumerable<string>> rows,
            IEnumerable<string> highlightedFlags)
        {
            var pRows = rows
                .PivotRows()
                .Append(highlightedFlags)
                .PivotRows();
            return pRows;
        }

        private static IEnumerable<string> GetHighlightedFlags(IEnumerable<RowData> rowData)
        {
            var highlightedFlags = rowData.Select(
                rd => rd.Values.All(
                    cd => cd.EffectiveFormat.BackgroundColor.Red.Equals(.8f)
                ).ToString()
            );
            return highlightedFlags;
        }
    }
}
