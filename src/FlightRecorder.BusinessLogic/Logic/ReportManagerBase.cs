using FlightRecorder.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Logic
{
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
        protected async Task<List<T>> GenerateReport<T>(string query) where T : class
        {
            var results = await _context.Set<T>().FromSqlRaw(query).ToListAsync();
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
                foreach (var placeHolder in placeHolderValues.Keys)
                {
                    content = content.Replace(placeHolder, placeHolderValues[placeHolder]);
                }
            }

            return content;
        }
    }
}
