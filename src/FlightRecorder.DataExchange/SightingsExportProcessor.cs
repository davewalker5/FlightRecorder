using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRecorder.DataExchange
{
    [ExcludeFromCodeCoverage]
    public class SightingsExportProcessor : BackgroundQueueProcessor<ExportWorkItem>
    {
        public SightingsExportProcessor(IBackgroundQueue<ExportWorkItem> queue, IServiceScopeFactory serviceScopeFactory) : base(queue, serviceScopeFactory)
        {
        }

        /// <summary>
        /// Export all the sightings from the database
        /// </summary>
        /// <param name="item"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected override async Task ProcessWorkItem(ExportWorkItem item, FlightRecorderFactory factory)
        {
            IEnumerable<Sighting> sightings = await factory.Sightings.ListAsync(null, 1, int.MaxValue).ToListAsync();
            CsvExporter exporter = new();
            exporter.Export(sightings, item.FileName);
        }
    }
}
