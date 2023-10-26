using FlightRecorder.Api.Entities;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Reporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace FlightRecorder.Api.Services
{
    public class ReportExportService : BackgroundQueueProcessor<ReportExportWorkItem>
    {
        private readonly AppSettings _settings;

        public ReportExportService(
            ILogger<BackgroundQueueProcessor<ReportExportWorkItem>> logger,
            IBackgroundQueue<ReportExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AppSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Export the content of a report
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItem(ReportExportWorkItem item, FlightRecorderFactory factory)
        {
            MessageLogger.LogInformation($"Exporting the {item.Type} report");

            switch (item.Type)
            {
                case ReportType.AirlineStatistics:
                    await ExportAirlineStatistics(factory, item);
                    break;
                case ReportType.LocationStatistics:
                    await ExportLocationStatistics(factory, item);
                    break;
                case ReportType.ManufacturerStatistics:
                    await ExportManufacturerStatistics(factory, item);
                    break;
                case ReportType.ModelStatistics:
                    await ExportModelStatistics(factory, item);
                    break;
                case ReportType.FlightsByMonth:
                    await ExportFlightsByMonth(factory, item);
                    break;
                default:
                    break;
            }

            MessageLogger.LogInformation($"{item.Type.ToString()} report export completed");
        }

        /// <summary>
        /// Export the airline statistics report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportAirlineStatistics(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.AirlineStatistics.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<AirlineStatistics>();
            exporter.Export(records, filePath, ',');
        }

        /// <summary>
        /// Export the location statistics report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportLocationStatistics(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.LocationStatistics.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<LocationStatistics>();
            exporter.Export(records, filePath, ',');
        }

        /// <summary>
        /// Export the manufacturer statistics report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportManufacturerStatistics(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.ManufacturerStatistics.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<ManufacturerStatistics>();
            exporter.Export(records, filePath, ',');
        }

        /// <summary>
        /// Export the model statistics report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportModelStatistics(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.ModelStatistics.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<ModelStatistics>();
            exporter.Export(records, filePath, ',');
        }

        /// <summary>
        /// Export the flights by month report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportFlightsByMonth(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.FlightsByMonth.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<FlightsByMonth>();
            exporter.Export(records, filePath, ',');
        }
    }
}