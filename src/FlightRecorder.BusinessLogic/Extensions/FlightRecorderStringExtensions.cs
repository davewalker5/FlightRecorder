using System;
using System.Text.RegularExpressions;

namespace FlightRecorder.BusinessLogic.Extensions
{
    public static class FlightRecorderStringExtensions
    {
        public static string CleanString(this string input)
        {
            return Regex.Replace(input, @"\t|\n|\r", "").Replace("  ", " ").Trim();
        }
    }
}
