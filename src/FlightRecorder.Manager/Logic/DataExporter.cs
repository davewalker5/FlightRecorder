using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.Entities.Reporting;
using FlightRecorder.Manager.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRecorder.Manager.Logic
{
    internal class DataExporter
    {
        private readonly FlightRecorderFactory _factory;

        public DataExporter(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Export the specified entities/report to the specified file
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task Export(CommandLineEntityType type, string fileName)
        {
            switch (type)
            {
                case CommandLineEntityType.sightings:
                    await ExportSightings(fileName);
                    break;
                case CommandLineEntityType.airports:
                    await ExportAirports(fileName);
                    break;
                case CommandLineEntityType.airlines:
                    await ExportAirlineStatistics(fileName);
                    break;
                case CommandLineEntityType.locations:
                    await ExportLocationStatistics(fileName);
                    break;
                case CommandLineEntityType.manufacturers:
                    await ExportManufacturerStatistics(fileName);
                    break;
                case CommandLineEntityType.models:
                    await ExportModelStatistics(fileName);
                    break;
                case CommandLineEntityType.flights:
                    await ExportFlightsByMonth(fileName);
                    break;
                default:
                    Console.WriteLine($"Invalid entity for data export: {type.ToString()}");
                    break;
            }
        }

        /// <summary>
        /// Export the sightings from the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportSightings(string fileName)
        {
            // The third argument to the "List" method is an arbitrarily large value intended
            // to return all records
            var sightings = await _factory.Sightings.ListAsync(null, 1, int.MaxValue).ToListAsync();
            SightingsExporter exporter = new();
            exporter.Export(sightings, fileName);
        }

        /// <summary>
        /// Export the airport details from the database
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportAirports(string fileName)
        {
            // The third argument to the "List" method is an arbitrarily large value intended
            // to return all records
            var airports = await _factory.Airports.ListAsync(null, 1, int.MaxValue).ToListAsync();
            AirportExporter exporter = new AirportExporter();
            exporter.Export(airports, fileName);
        }

        /// <summary>
        /// Export the airline statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportAirlineStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await _factory.AirlineStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<AirlineStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the location statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportLocationStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await _factory.LocationStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<LocationStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the manufacturer statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportManufacturerStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await _factory.ManufacturerStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<ManufacturerStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the model statistics report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportModelStatistics(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await _factory.ModelStatistics.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<ModelStatistics>();
            exporter.Export(records, fileName, ',');
        }

        /// <summary>
        /// Export the flights by month report
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task ExportFlightsByMonth(string fileName)
        {
            // The third argument to the report generation method is an arbitrarily large value intended
            // to return all records
            var records = await _factory.FlightsByMonth.GenerateReportAsync(null, null, 1, int.MaxValue);
            var exporter = new CsvExporter<FlightsByMonth>();
            exporter.Export(records, fileName, ',');
        }
    }
}
