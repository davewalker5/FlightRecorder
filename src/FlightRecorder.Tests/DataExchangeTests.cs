using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.DataExchange.Export;
using FlightRecorder.DataExchange.Import;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Tests.RandomGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class DataExchangeTests
    {
        private const string LocationName = "Murcia Corvera International Airport";

        private const string FlightNumber = "U28551";
        private const string Embarkation = "LGW";
        private const string Destination = "RMU";
        private const string AirlineName = "EasyJet";

        private const string ModelName = "A319-111";
        private const string ManufacturerName = "Airbus";
        private const string Registration = "G-EZFY";
        private const string SerialNumber = "4418";
        private const string Age = "9";

        private const long Altitude = 930;
        private readonly DateTime SightingDate = new DateTime(2019, 9, 22);
        private const bool IsMyFlight = true;

        private FlightRecorderFactory _factory;
        private long _sightingId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _sightingId = Task.Run(() => _factory.Sightings.AddAsync(new FlattenedSighting
            {
                FlightNumber = FlightNumber,
                Airline = AirlineName,
                Registration = Registration,
                SerialNumber = SerialNumber,
                Manufacturer = ManufacturerName,
                Model = ModelName,
                Age = Age,
                Embarkation = Embarkation,
                Destination = Destination,
                Altitude = Altitude,
                Date = SightingDate,
                Location = LocationName,
                IsMyFlight = IsMyFlight
            })).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.Id == _sightingId);

            Assert.IsNotNull(sighting);
            Assert.AreEqual(Altitude, sighting.Altitude);
            Assert.AreEqual(SightingDate, sighting.Date);

            Assert.IsNotNull(sighting.Location);
            Assert.AreEqual(LocationName, sighting.Location.Name);

            Assert.IsNotNull(sighting.Flight);
            Assert.AreEqual(FlightNumber, sighting.Flight.Number);
            Assert.AreEqual(Embarkation, sighting.Flight.Embarkation);
            Assert.AreEqual(Destination, sighting.Flight.Destination);

            Assert.IsNotNull(sighting.Flight.Airline);
            Assert.AreEqual(AirlineName, sighting.Flight.Airline.Name);

            Assert.IsNotNull(sighting.Aircraft);
            Assert.AreEqual(Registration, sighting.Aircraft.Registration);
            Assert.AreEqual(SerialNumber, sighting.Aircraft.SerialNumber);
            Assert.AreEqual(DateTime.Now.Year - long.Parse(Age), sighting.Aircraft.Manufactured);

            Assert.IsNotNull(sighting.Aircraft.Model);
            Assert.AreEqual(ModelName, sighting.Aircraft.Model.Name);

            Assert.IsNotNull(sighting.Aircraft.Model.Manufacturer);
            Assert.AreEqual(ManufacturerName, sighting.Aircraft.Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task FlattenCollectionTest()
        {
            IEnumerable<Sighting> sightings = await _factory.Sightings.ListAsync(null, 1, 100).ToListAsync();
            IEnumerable<FlattenedSighting> flattened = sightings.Flatten();
            Assert.AreEqual(1, flattened.Count());
            Assert.AreEqual(FlightNumber, flattened.First().FlightNumber);
            Assert.AreEqual(AirlineName, flattened.First().Airline);
            Assert.AreEqual(Registration, flattened.First().Registration);
            Assert.AreEqual(SerialNumber, flattened.First().SerialNumber);
            Assert.AreEqual(ManufacturerName, flattened.First().Manufacturer);
            Assert.AreEqual(ModelName, flattened.First().Model);
            Assert.AreEqual(Age, flattened.First().Age);
            Assert.AreEqual(Embarkation, flattened.First().Embarkation);
            Assert.AreEqual(Destination, flattened.First().Destination);
            Assert.AreEqual(Altitude, flattened.First().Altitude);
            Assert.AreEqual(SightingDate, flattened.First().Date);
            Assert.AreEqual(LocationName, flattened.First().Location);
            Assert.IsTrue(flattened.First().IsMyFlight);
        }

        [TestMethod]
        public void ExportSightingsTest()
        {
            var sightings = FlightRecorderGenerators.GenerateListOfRandomSightings(10);

            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            new SightingsExporter().Export(sightings, filePath);

            var info = new FileInfo(filePath);
            Assert.AreEqual(info.FullName, filePath);
            Assert.IsTrue(info.Length > 0);
            File.Delete(filePath);
        }

        [TestMethod]
        public async Task ImportSightingsTest()
        {
            var sightings = FlightRecorderGenerators.GenerateListOfRandomSightings(10);
            var aircraft = sightings.Select(x => x.Aircraft).Distinct();
            var models = sightings.Select(x => x.Aircraft.Model).Distinct();
            var manufacturers = sightings.Select(x => x.Aircraft.Model.Manufacturer).Distinct();
            var flights = sightings.Select(x => x.Flight).Distinct();
            var locations = sightings.Select(x => x.Location).Distinct();

            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            new SightingsExporter().Export(sightings, filePath);

            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            FlightRecorderFactory factory = new FlightRecorderFactory(context);
            await new SightingsImporter().Import(filePath, factory);

            File.Delete(filePath);

            var imported = await factory.Sightings.ListAsync(x => true, 1, 1000000).ToListAsync();
            var importedAircraft = imported.Select(x => x.Aircraft).Distinct();
            var importedModels = imported.Select(x => x.Aircraft.Model).Distinct();
            var importedManufacturers = imported.Select(x => x.Aircraft.Model.Manufacturer).Distinct();
            var importedFlights = imported.Select(x => x.Flight).Distinct();
            var importedLocations = imported.Select(x => x.Location).Distinct();

            Assert.IsNotNull(imported);
            Assert.AreEqual(sightings.Count, imported.Count);
            Assert.AreEqual(aircraft.Count(), importedAircraft.Count());
            Assert.AreEqual(models.Count(), importedModels.Count());
            Assert.AreEqual(manufacturers.Count(), importedManufacturers.Count());
            Assert.AreEqual(flights.Count(), importedFlights.Count());
            Assert.AreEqual(locations.Count(), importedLocations.Count());
        }

        [TestMethod]
        public void ExportAirportsTest()
        {
            var airports = FlightRecorderGenerators.GenerateListOfRandomAirports(10);

            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            new AirportExporter().Export(airports, filePath);

            var info = new FileInfo(filePath);
            Assert.AreEqual(info.FullName, filePath);
            Assert.IsTrue(info.Length > 0);

            File.Delete(filePath);
        }

        [TestMethod]
        public async Task ImportAirportsTest()
        {
            var airports = FlightRecorderGenerators.GenerateListOfRandomAirports(10);
            var countries = airports.Select(x => x.Country).Distinct();
            var filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            new AirportExporter().Export(airports, filePath);

            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            FlightRecorderFactory factory = new FlightRecorderFactory(context);
            await new AirportImporter().Import(filePath, factory);

            File.Delete(filePath);

            var importedAirports = await factory.Airports.ListAsync(x => true, 1, 1000000).ToListAsync();
            Assert.IsNotNull(importedAirports);
            Assert.AreEqual(airports.Count, importedAirports.Count);

            var importedCountries = await factory.Countries.ListAsync(x => true, 1, 1000000).ToListAsync();
            Assert.IsNotNull(importedCountries);
            Assert.AreEqual(countries.Count(), importedCountries.Count);
        }
    }
}
