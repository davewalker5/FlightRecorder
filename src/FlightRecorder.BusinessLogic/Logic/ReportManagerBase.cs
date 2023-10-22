using FlightRecorder.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Logic
{
    [ExcludeFromCodeCoverage]
    internal abstract class ReportManagerBase
    {
        private readonly FlightRecorderDbContext _context;

        protected ReportManagerBase(FlightRecorderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generate the report for entity type T given a query that returns rows mapping to that
        /// type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<T>> GenerateReport<T>(string query, int pageNumber, int pageSize) where T : class
        {
            // Pagination using Skip and Take causes the database query to fail with FromSqlRaw, possible
            // dependent on the DBS. To avoid this, the results are queried in two steps:
            //
            // 1) Query the database for all the report results and convert to a list
            // 2) Extract the required page from the in-memory list
            var all = await _context.Set<T>().FromSqlRaw(query).ToListAsync();
            var results = all.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return results;
        }

        /// <summary>
        /// Get the full path to a file held in the SQL sub-folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected static string GetSqlFilePath(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyFolder = Path.GetDirectoryName(assembly.Location);
            var sqlFilePath = Path.Combine(assemblyFolder, "sql", fileName);
            return sqlFilePath;
        }

        /// <summary>
        /// Read a SQL file and perform placeholder value replacement
        /// </summary>
        /// <param name="file"></param>
        /// <param name="placeHolderValues"></param>
        protected static string ReadSqlFile(string file, Dictionary<string, string> placeHolderValues)
        {
            string content = "";

            // Open a stream reader to read the file content
            using (var reader = new StreamReader(file))
            {
                // Read the file content
                content = reader.ReadToEnd();

                // Perform place holder replacement
                content = ReplacePlaceHolders(content, placeHolderValues);
            }

            return content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="placeHolderValues"></param>
        /// <returns></returns>
        protected static string ReadSqlResource(string file, Dictionary<string, string> placeHolderValues)
        {
            string content = "";

            // Get the name of the resource and a resource stream for reading it
            var assembly = Assembly.GetExecutingAssembly();
            var sqlResourceName = $"FlightRecorder.BusinessLogic.Sql.{file}";
            var resourceStream = assembly.GetManifestResourceStream(sqlResourceName);

            // Open a stream reader to read the file content
            using (var reader = new StreamReader(resourceStream))
            {
                // Read the file content
                content = reader.ReadToEnd();

                // Perform place holder replacement
                content = ReplacePlaceHolders(content, placeHolderValues);
            }

            return content;
        }

        /// <summary>
        /// Perform place holder replacement on content read from an embedded resource or SQL file
        /// </summary>
        /// <param name="content"></param>
        /// <param name="placeHolderValues"></param>
        /// <returns></returns>
        private static string ReplacePlaceHolders(string content, Dictionary<string, string> placeHolderValues)
        {
            foreach (var placeHolder in placeHolderValues.Keys)
            {
                content = content.Replace(placeHolder, placeHolderValues[placeHolder]);
            }

            return content;
        }
    }
}
