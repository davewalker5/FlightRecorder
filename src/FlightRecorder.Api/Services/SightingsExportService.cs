using FlightRecorder.Api.Entities;
using FlightRecorder.Api.Interfaces;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Api.Services
{
    [ExcludeFromCodeCoverage]
    public class SightingsExportService : BackgroundQueueProcessor<SightingsExportWorkItem>
    {
        private readonly FlightRecorderApplicationSettings _settings;

        public SightingsExportService(
            ILogger<BackgroundQueueProcessor<SightingsExportWorkItem>> logger,
            IBackgroundQueue<SightingsExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            FlightRecorderApplicationSettings settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings;
        }

        /// <summary>
        /// Export all the sightings from the database
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItemAsync(SightingsExportWorkItem item, FlightRecorderFactory factory)
        {
            // Get the list of sightings to export
            MessageLogger.LogInformation("Retrieving sightings for export");
            List<Sighting> sightings = await factory.Sightings
                                                    .ListAsync(null, 1, int.MaxValue)
                                                    .ToListAsync();

            // Get the full path to the export file
            var filePath = Path.Combine(_settings.SightingsExportPath, item.FileName);

            // Export the sightings
            MessageLogger.LogInformation($"Exporting {sightings.Count} sightings to {filePath}");
            SightingsExporter exporter = new();
            exporter.Export(sightings, filePath);
            MessageLogger.LogInformation("Sightings export completed");
        }
    }
}
