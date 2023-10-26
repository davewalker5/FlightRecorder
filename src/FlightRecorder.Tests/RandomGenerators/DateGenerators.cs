using System;

namespace FlightRecorder.Tests.RandomGenerators
{
    public static class DateGenerators
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Generate a random date in the specified year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GenerateRandomDate(int year)
        {
            var baseDate = new DateTime(year, 1, 1, 0, 0, 0);
            var date = baseDate.AddDays(_random.Next(1, 366));
            return date;
        }

        /// <summary>
        /// Return a random year
        /// </summary>
        /// <param name="baseYear"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int GenerateRandomYear(int baseYear, int range)
        {
            return baseYear + _random.Next(range);
        }
    }
}
