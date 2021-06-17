using System;
using System.Collections.Generic;
using System.Linq;

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
                foreach (var cell in row) Console.Write($"{cell.PadRight(padding)} | ");
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
    }
}
