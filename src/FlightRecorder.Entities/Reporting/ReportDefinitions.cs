using System.Collections.Generic;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Reporting
{
    public static class ReportDefinitions
    {
        public static List<ReportDefinition> Definitions { get; } =
        [
            new ReportDefinition(ReportType.AirlineStatistics, typeof(AirlineStatistics), "Airline Statistics"),
            new ReportDefinition(ReportType.LocationStatistics, typeof(LocationStatistics), "Location Statistics"),
            new ReportDefinition(ReportType.ManufacturerStatistics, typeof(ManufacturerStatistics), "Manufacturer Statistics"),
            new ReportDefinition(ReportType.ModelStatistics, typeof(ModelStatistics), "Model Statistics"),
            new ReportDefinition(ReportType.FlightsByMonth, typeof(FlightsByMonth), "Flights By Month"),
            new ReportDefinition(ReportType.JobStatus, typeof(JobStatus), "Job Status"),
            new ReportDefinition(ReportType.MyFlights, typeof(MyFlights), "My Flights")
        ];
    }
}
