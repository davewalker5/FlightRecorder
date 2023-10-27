using FlightRecorder.Api.Entities;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Db;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRecorder.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class SightingsExportService : BackgroundQueueProcessor<SightingsExportWorkItem>
    {
        private readonly AppSettings _settings;

        public SightingsExportService(
            ILogger<BackgroundQueueProcessor<SightingsExportWorkItem>> logger,
            IBackgroundQueue<SightingsExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AppSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Export all the sightings from the database
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItem(SightingsExportWorkItem item, FlightRecorderFactory factory)
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
