using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;

namespace CautionaryAlertsApi.V1.Helpers
{
    public static class GoogleSheetHelpers
    {
        public static IEnumerable<IEnumerable<T>> PivotRows<T>(this IEnumerable<IEnumerable<T>> source)
        {
            var enumerators = source.Select(e => e.GetEnumerator()).ToArray();

            try
            {
                while (enumerators.All(e => e.MoveNext()))
                    yield return enumerators
                        .Select(e => e.Current)
                        .ToArray();
            }
            finally
            {
                Array.ForEach(enumerators, e => e.Dispose());
            }
        }

        public static void PrintSpreadSheet(IEnumerable<IEnumerable<string>> rows)
        {
            var padding = (
                from row in rows
                from string cell in row
                select cell.Length
            ).Prepend(0).Max();

            foreach (var row in rows.ToArray())
            {
                Console.Write("| ");
                foreach (var cell in row) Console.Write($"{cell} | ");
                Console.WriteLine();
            }
        }

        public static bool IsValid<TEntity>(ICollection<string> headers)
        {
            var props = typeof(TEntity)
                .GetProperties()
                .Where(prop => prop.Name != "IsHighlighted");
            return props.All(prop => headers.Contains(prop.Name));
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

        public static IEnumerable<string> GetHighlightedFlags(IEnumerable<RowData> rowData, Color highlightColor)
        {
            var highlightedFlags = rowData
                .Select(rd => rd.Values
                    .All(cd => cd.EffectiveFormat != null &&
                               cd.EffectiveFormat.BackgroundColor.Red.Equals(highlightColor.Red) &&
                               cd.EffectiveFormat.BackgroundColor.Green.Equals(highlightColor.Green) &&
                               cd.EffectiveFormat.BackgroundColor.Blue.Equals(highlightColor.Blue)
                    ).ToString()
                );

            return highlightedFlags;
        }
    }
}
