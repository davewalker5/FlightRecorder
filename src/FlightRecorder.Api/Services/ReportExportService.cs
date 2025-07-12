using FlightRecorder.Api.Entities;
using FlightRecorder.Api.Interfaces;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Reporting;

namespace FlightRecorder.Api.Services
{
    public class ReportExportService : BackgroundQueueProcessor<ReportExportWorkItem>
    {
        private readonly FlightRecorderApplicationSettings _settings;

        public ReportExportService(
            ILogger<BackgroundQueueProcessor<ReportExportWorkItem>> logger,
            IBackgroundQueue<ReportExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            FlightRecorderApplicationSettings settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings;
        }

        /// <summary>
        /// Export the content of a report
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(ReportExportWorkItem item, FlightRecorderFactory factory)
        {
            MessageLogger.LogInformation($"Exporting the {item.Type} report");

            switch (item.Type)
            {
                case ReportType.AirlineStatistics:
                    await ExportAirlineStatisticsAsync(factory, item);
                    break;
                case ReportType.LocationStatistics:
                    await ExportLocationStatisticsAsync(factory, item);
                    break;
                case ReportType.ManufacturerStatistics:
                    await ExportManufacturerStatisticsAsync(factory, item);
                    break;
                case ReportType.ModelStatistics:
                    await ExportModelStatisticsAsync(factory, item);
                    break;
                case ReportType.FlightsByMonth:
                    await ExportFlightsByMonthAsync(factory, item);
                    break;
                case ReportType.JobStatus:
                    await ExportJobStatusAsync(factory, item);
                    break;
                case ReportType.MyFlights:
                    await ExportMyFlightsAsync(factory, item);
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
        private async Task ExportAirlineStatisticsAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
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
        private async Task ExportLocationStatisticsAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
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
        private async Task ExportManufacturerStatisticsAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
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
        private async Task ExportModelStatisticsAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
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
        private async Task ExportFlightsByMonthAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.FlightsByMonth.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<FlightsByMonth>();
            exporter.Export(records, filePath, ',');
        }

        /// <summary>
        /// Export the job status report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportJobStatusAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.JobStatuses
                                       .ListAsync(x => ((item.Start == null) || (x.Start >= item.Start)) &&
                                                       ((item.End == null) || (x.End == null) || (x.End <= item.End)),
                                                  1,
                                                  int.MaxValue)
                                       .ToListAsync();
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<JobStatus>();
            exporter.Export(records, filePath, ',');
        }

        /// <summary>
        /// Export the "My Flights" report
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task ExportMyFlightsAsync(FlightRecorderFactory factory, ReportExportWorkItem item)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await factory.MyFlights.GenerateReportAsync(item.Start, item.End, 1, int.MaxValue);
            var filePath = Path.Combine(_settings.ReportsExportPath, item.FileName);
            var exporter = new CsvExporter<MyFlights>();
            exporter.Export(records, filePath, ',');
        }
    }
}