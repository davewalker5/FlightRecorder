using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class SightingManagerTest
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
        private const long YearOfManufacture = 2010;

        private const long Altitude = 930;
        private readonly DateTime SightingDate = new DateTime(2019, 9, 22);
        private const bool IsMyFlight = true;

        private FlightRecorderFactory _factory;
        private long _locationId;
        private long _airlineId;
        private long _flightId;
        private long _modelId;
        private long _aircraftId;
        private long _sightingId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context, new MockFileLogger());

            _locationId = Task.Run(() => _factory.Locations.AddAsync(LocationName)).Result.Id;
            _airlineId = Task.Run(() => _factory.Airlines.AddAsync(AirlineName)).Result.Id;
            _flightId = Task.Run(() => _factory.Flights.AddAsync(FlightNumber, Embarkation, Destination, _airlineId)).Result.Id;
            _modelId = Task.Run(() => _factory.Models.AddAsync(ModelName, ManufacturerName)).Result.Id;
            _aircraftId = Task.Run(() => _factory.Aircraft.AddAsync(Registration, SerialNumber, YearOfManufacture, _modelId)).Result.Id;
            _sightingId = Task.Run(() => _factory.Sightings.AddAsync(Altitude, SightingDate, _locationId, _flightId, _aircraftId, IsMyFlight)).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.Id == _sightingId);

            Assert.IsNotNull(sighting);
            Assert.AreEqual(_sightingId, sighting.Id);
            Assert.AreEqual(Altitude, sighting.Altitude);
            Assert.AreEqual(SightingDate, sighting.Date);
            Assert.IsTrue(sighting.IsMyFlight);

            Assert.IsNotNull(sighting.Location);
            Assert.AreEqual(_locationId, sighting.Location.Id);
            Assert.AreEqual(LocationName, sighting.Location.Name);

            Assert.IsNotNull(sighting.Flight);
            Assert.AreEqual(_flightId, sighting.Flight.Id);
            Assert.AreEqual(FlightNumber, sighting.Flight.Number);
            Assert.AreEqual(Embarkation, sighting.Flight.Embarkation);
            Assert.AreEqual(Destination, sighting.Flight.Destination);

            Assert.IsNotNull(sighting.Flight.Airline);
            Assert.AreEqual(AirlineName, sighting.Flight.Airline.Name);

            Assert.IsNotNull(sighting.Aircraft);
            Assert.AreEqual(_aircraftId, sighting.Aircraft.Id);
            Assert.AreEqual(Registration, sighting.Aircraft.Registration);
            Assert.AreEqual(SerialNumber, sighting.Aircraft.SerialNumber);
            Assert.AreEqual(YearOfManufacture, sighting.Aircraft.Manufactured);

            Assert.IsNotNull(sighting.Aircraft.Model);
            Assert.AreEqual(ModelName, sighting.Aircraft.Model.Name);

            Assert.IsNotNull(sighting.Aircraft.Model.Manufacturer);
            Assert.AreEqual(ManufacturerName, sighting.Aircraft.Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.FlightId == 0);
            Assert.IsNull(sighting);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(null, 1, 100)
                                                     .ToListAsync();
            Assert.AreEqual(1, sightings.Count);
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public async Task ListByAircraftAsyncTest()
        {
            IAsyncEnumerable<Sighting> matches = await _factory.Sightings
                                                               .ListByAircraftAsync(Registration, 1, 100);
            List<Sighting> sightings = await matches.ToListAsync();
            Assert.AreEqual(1, sightings.Count);
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public async Task ListByAircraftWithNoSightingsAsyncTest()
        {
            await _factory.Aircraft.AddAsync("G-EZEH", "2184", 2004, _modelId);
            List<Sighting> sightings = await _factory.Sightings.ListByAircraftAsync("G-EZEH", 1, 100).Result.ToListAsync();
            Assert.AreEqual(0, sightings.Count);
        }

        [TestMethod]
        public async Task ListByMissingAircraftAsyncTest()
        {
            IAsyncEnumerable<Sighting> sightings = await _factory.Sightings.ListByAircraftAsync("Missing", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public async Task ListByRouteAsyncTest()
        {
            IAsyncEnumerable<Sighting> matches = await _factory.Sightings
                                                               .ListByRouteAsync(Embarkation, Destination, 1, 100);
            List<Sighting> sightings = await matches.ToListAsync();
            Assert.AreEqual(1, sightings.Count);
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public async Task ListByRouteWithNoSightingsAsyncTest()
        {
            await _factory.Flights.AddAsync("BA92", "YYZ", "LHR", _airlineId);
            List<Sighting> sightings = await _factory.Sightings.ListByRouteAsync("YYZ", "LHR", 1, 100).Result.ToListAsync();
            Assert.AreEqual(0, sightings.Count);
        }

        [TestMethod]
        public async Task ListByMissingRouteAsyncTest()
        {
            IAsyncEnumerable<Sighting> sightings = await _factory.Sightings.ListByRouteAsync("RMU", "MAN", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public async Task ListByAirlineAsyncTest()
        {
            IAsyncEnumerable<Sighting> matches = await _factory.Sightings
                                                               .ListByAirlineAsync(AirlineName, 1, 100);
            List<Sighting> sightings = await matches.ToListAsync();
            Assert.AreEqual(1, sightings.Count);
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public async Task ListByAirlineWithNoSightingsAsyncTest()
        {
            await _factory.Airlines.AddAsync("British Airways");
            IAsyncEnumerable<Sighting> sightings = await _factory.Sightings
                                                                 .ListByAirlineAsync("British Airways", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public async Task ListByMissingAirline()
        {
            IAsyncEnumerable<Sighting> sightings = await _factory.Sightings
                                                                 .ListByAirlineAsync("Missing", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public async Task ListByLocationAsyncTest()
        {
            IAsyncEnumerable<Sighting> matches = await _factory.Sightings
                                                               .ListByLocationAsync(LocationName, 1, 100);
            List<Sighting> sightings = await matches.ToListAsync();
            Assert.AreEqual(1, sightings.Count);
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public async Task ListByLocationWithNoSightingsAsyncTest()
        {
            await _factory.Airlines.AddAsync("Gatwick Airport");
            IAsyncEnumerable<Sighting> sightings = await _factory.Sightings
                                                                 .ListByLocationAsync("Gatwick Airport", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public async Task ListByMissingLocation()
        {
            IAsyncEnumerable<Sighting> sightings = await _factory.Sightings
                                                                 .ListByLocationAsync("Missing", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public async Task GetMostRecentFlightSighting()
        {
            var sighting = await _factory.Sightings.GetMostRecent(x => x.Flight.Number == FlightNumber);
            Assert.IsNotNull(sighting);
            Assert.AreEqual(FlightNumber, sighting.Flight.Number);
            Assert.AreEqual(SightingDate, sighting.Date);
        }

        [TestMethod]
        public async Task GetMostRecentAircraftSighting()
        {
            var sighting = await _factory.Sightings.GetMostRecent(x => x.Aircraft.Registration == Registration);
            Assert.IsNotNull(sighting);
            Assert.AreEqual(Registration, sighting.Aircraft.Registration);
            Assert.AreEqual(SightingDate, sighting.Date);
        }

        [TestMethod]
        public async Task GetMissingRecentSighting()
        {
            var sighting = await _factory.Sightings.GetMostRecent(x => x.Flight.Embarkation == "Missing");
            Assert.IsNull(sighting);
        }
    }
}
