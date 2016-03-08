using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steego.LinqPad.Formatting {
    public static class FormattingExtensions {
        public static string ToJiraTable<T>(this IEnumerable<T> sequence) {
            var properties = (from prop in typeof(T).GetProperties()
                              select prop).ToArray();

            var lines = new List<string> { MarkdowRow(properties.Select(p => p.Name), "||") };

            foreach(var row in sequence) {
                lines.Add(MarkdowRow((from p in properties
                                      select Markdown(p.GetValue(row, new object[] { }))).ToArray(), "|"));
            }

            return String.Join("\n", lines.ToArray());
        }

        private static string MarkdowRow(IEnumerable<string> values, string delimiter) {
            return delimiter + " " + String.Join(" " + delimiter + " ", values.ToArray()) + " " + delimiter;
        }

        private static string Markdown(object o) {
            if(o == null)
                return "";
            if(o is bool)
                return ((bool) o == true) ? "Yes" : "No";
            if(o is string)
                return o.ToString();
            return o.ToString();
        }
    }
}
