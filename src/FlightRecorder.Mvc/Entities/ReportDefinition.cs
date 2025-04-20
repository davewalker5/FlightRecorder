using System;

namespace FlightRecorder.Mvc.Entities
{
    public class ReportDefinition
    {
        /// <summary>
        /// The report type passed to the API to generate a report of this type
        /// </summary>
        public ReportType ReportType { get; set; }

        /// <summary>
        /// Report row entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Human-readable report name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Prefix for exported file names
        /// </summary>
        public string FilePrefix { get { return DisplayName.Replace(" ", ""); } }

        public ReportDefinition(ReportType reportType, Type entityType, string name)
        {
            ReportType = reportType;
            EntityType = entityType;
            DisplayName = name;
        }
    }
}
