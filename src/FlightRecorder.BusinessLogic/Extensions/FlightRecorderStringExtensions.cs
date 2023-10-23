using System;
using System.Text.RegularExpressions;

namespace FlightRecorder.BusinessLogic.Extensions
{
    public static partial class FlightRecorderStringExtensions
    {
        public static string CleanString(this string input)
        {
            return MyRegex().Replace(input ?? "", "").Replace("  ", " ").Trim();
        }

        [GeneratedRegex("\\t|\\n|\\r")]
        private static partial Regex MyRegex();
    }
}
