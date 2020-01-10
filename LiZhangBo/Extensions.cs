using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiZhangBo
{
    static class Extensions
    {
        public static string WithSwitch(this string value, string name)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return $" {name} {value}";
        }

        public static double? ParseSize(this string from)
        {
            if (string.IsNullOrEmpty(from))
                return null;
            var sizeTable = new List<char>() { 'k', 'M', 'G' };
            char? suffix = from.Last();
            if (Enumerable.Range(0, 9).Select(i => Convert.ToChar(i)).Contains(suffix.Value))
                suffix = null;
            if (suffix.HasValue && !sizeTable.Contains(suffix.Value))
                throw new ArgumentException("bad size suffix");
            var value = double.Parse(from.Substring(0, from.Length - 1));
            if (suffix.HasValue)
                value *= Math.Pow(10, (3 * (sizeTable.IndexOf(suffix.Value) + 1)));
            return value;
        }

        public static TimeSpan? ParseToTimeSpan(this string from)
        {
            if (string.IsNullOrEmpty(from))
                return null;
            return TimeSpan.Parse(from);
        }
    }
}
