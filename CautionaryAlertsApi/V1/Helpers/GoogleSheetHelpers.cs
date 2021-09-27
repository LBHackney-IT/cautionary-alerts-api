using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;

namespace CautionaryAlertsApi.V1.Helpers
{
    public static class GoogleSheetHelpers
    {
        public static IEnumerable<ValueIndexPair> ExcludeInvalidPropertyRefs(this IReadOnlyCollection<string> pRefs)
        {
            var goodPropertyRefs = pRefs
                .Select((value, index) => new ValueIndexPair(value, index))
                .Where(p => p.Value.Length > 0 && p.Value != "Not found" && p.Value != "#REF!")
                .ToList();

            var badPropertyRefs = pRefs
                .Select((value, index) => new ValueIndexPair(value, index))
                .Except(goodPropertyRefs);

            Console.WriteLine(
                $"{badPropertyRefs.Count()} rows contain invalid property references and were excluded from the result.");

            return goodPropertyRefs;
        }

        public static List<string> GetAllPropertyRefs(this ValueRange data)
        {
            var pRefs = data.Values.First().Select(value => value.ToString()).ToList();
            return pRefs;
        }
    }
        public class ValueIndexPair
        {
            public ValueIndexPair(string v, int i) { Value = v; Index = i; }
            public string Value { get; }
            public int Index { get; }
        }

}
