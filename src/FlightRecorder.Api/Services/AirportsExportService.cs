using FlightRecorder.Api.Entities;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Db;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRecorder.Api.Services
{
    public class AirportsExportService : BackgroundQueueProcessor<AirportsExportWorkItem>
    {
        private readonly AppSettings _settings;

        public AirportsExportService(
            ILogger<BackgroundQueueProcessor<AirportsExportWorkItem>> logger,
            IBackgroundQueue<AirportsExportWorkItem> queue,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AppSettings> settings)
            : base(logger, queue, serviceScopeFactory)
        {
            _settings = settings.Value;
        }

        /// <summary>
        /// Export all the airports from the database
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItem(AirportsExportWorkItem item, FlightRecorderFactory factory)
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