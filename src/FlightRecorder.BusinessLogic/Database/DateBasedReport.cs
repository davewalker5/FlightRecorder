using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Database
{
    [ExcludeFromCodeCoverage]
    internal class DateBasedReport<T> : ReportManagerBase, IDateBasedReport<T> where T : class
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string FromDatePlaceHolder = "$from";
        private const string ToDatePlaceHolder = "$to";

        internal DateBasedReport(FlightRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Generate a datebased report for reporting entity type T
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GenerateReportAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // SQL report files are named after the keyless entity type they map to with a .sql extension
            var sqlFile = $"{typeof(T).Name}.sql";

            // Load the SQL file and perform date range place-holder replacements
            var query = ReadDateBasedSqlReportResource(sqlFile, from, to);

            // Run the query and return the results
            var results = await GenerateReportAsync<T>(query, pageNumber, pageSize);
            return results;
        }

        /// <summary>
        /// Read the SQL report file for a sightings-based report with a date range in it
        /// </summary>
        /// <param name="reportFile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static string ReadDateBasedSqlReportResource(string reportFile, DateTime? from, DateTime? to)
        {
            // Get non-NULL versions of the from and to dates
            var nonNullFromDate = (from ?? DateTime.MinValue).ToString(DateFormat);
            var nonNullToDate = (to ?? DateTime.MaxValue).ToString(DateFormat);

            // Read and return the query, replacing the date range parameters
            var query = ReadSqlResource(reportFile, new Dictionary<string, string>
            {
                { FromDatePlaceHolder, nonNullFromDate },
                { ToDatePlaceHolder, nonNullToDate }

            });

            return query;
        }

    }
}
