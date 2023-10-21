using FlightRecorder.Data;
using FlightRecorder.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class DateBasedReport<T> : ReportManagerBase, IDateBasedReport<T> where T : class
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string FromDatePlaceHolder = "$from";
        private const string ToDatePlaceHolder = "$to";

        internal DateBasedReport(FlightRecorderDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Generate the airline statistics report
        /// </summary>
        /// <param name="file"></param>
        public async Task<List<T>> GenerateReport(DateTime? from, DateTime? to)
        {
            // SQL report files are named after the keyless entity type they map to with a .sql extension
            var sqlFile = $"{typeof(T).Name}.sql";

            // Load the SQL file and perform date range place-holder replacements
            var query = ReadDateBasedSqlFile(sqlFile, from, to);

            // Run the query and return the results
            var results = await GenerateReport<T>(query);
            return results;
        }

        /// <summary>
        /// Read the SQL report file for a sightings-based report with a date range in it
        /// </summary>
        /// <param name="reportFile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static string ReadDateBasedSqlFile(string reportFile, DateTime? from, DateTime? to)
        {
            // Get the full path to the SQL report file
            var filePath = GetSqlFilePath(reportFile);

            // Get non-NULL versions of the from and to dates
            var nonNullFromDate = (from ?? DateTime.MinValue).ToString(DateFormat);
            var nonNullToDate = (to ?? DateTime.MaxValue).ToString(DateFormat);

            // Read and return the query, replacing the date range parameters
            var query = ReadSqlFile(filePath, new Dictionary<string, string>
            {
                { FromDatePlaceHolder, nonNullFromDate },
                { ToDatePlaceHolder, nonNullToDate }

            });

            return query;
        }

    }
}
