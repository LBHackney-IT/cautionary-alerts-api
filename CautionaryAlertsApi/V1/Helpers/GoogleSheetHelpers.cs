using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;

namespace CautionaryAlertsApi.V1.Helpers
{
    public static class GoogleSheetHelpers
    {
        public static IEnumerable<ValueIndexPair> ExcludeInvalidCellValues(this IReadOnlyCollection<string> cellValues)
        {
            return cellValues
                .Select((value, index) => new ValueIndexPair(value, index))
                .Where(p => p.Value.Length > 0 && p.Value != "Not found" && p.Value != "#REF!")
                .ToList();
        }

        public static List<string> GetCellValues(this ValueRange data)
        {
            // valueRange contains list of columns, for now assume that it always contains just single column
            return data.Values
                .First()
                .Select(value => value.ToString()).ToList();
        }
    }
    public class ValueIndexPair
    {
        public ValueIndexPair(string v, int i) { Value = v; Index = i; }
        public string Value { get; }
        public int Index { get; }
    }

}
