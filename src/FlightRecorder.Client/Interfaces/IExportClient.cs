using System;
using System.Threading.Tasks;
using FlightRecorder.Entities.Reporting;

namespace FlightRecorder.Client.Interfaces
{
    public interface IExportClient
    {
        Task ExportSightings(string fileName);
        Task ExportAirports(string fileName);
        Task ExportReport(ReportType type, DateTime? from, DateTime? to, string fileName);
        Task ExportReport<T>(string from, string to) where T : class;
    }
}
