using FlightRecorder.Api.Entities;
using FlightRecorder.Api.Interfaces;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Api.Services
{
    public class AirportsExportService : BackgroundQueueProcessor<AirportsExportWorkItem>
    {
        private readonly FlightRecorderApplicationSettings _settings;

        public AirportsExportService(
            ILogger<BackgroundQueueProcessor<AirportsExportWorkItem>> logger,
            IBackgroundQueue<AirportsExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            FlightRecorderApplicationSettings settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings;
        }

        /// <summary>
        /// Export all the airports from the database
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(AirportsExportWorkItem item, FlightRecorderFactory factory)
        {
            // Get the list of sightings to export
            MessageLogger.LogInformation("Retrieving airports for export");
            List<Airport> airports = await factory.Airports
                                                   .ListAsync(null, 1, int.MaxValue)
                                                   .ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.AirportsExportPath, item.FileName);

            // Export the sightings
            MessageLogger.LogInformation($"Exporting {airports.Count} airports to {filePath}");
            AirportExporter exporter = new();
            exporter.Export(airports, filePath);
            MessageLogger.LogInformation("Airports export completed");
        }
    }
}