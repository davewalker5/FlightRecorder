using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
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
        private readonly DateTime AsyncSightingDate = new DateTime(2020, 4, 11);

        private FlightRecorderFactory _factory;
        private long _locationId;
        private long _flightId;
        private long _aircraftId;
        private long _sightingId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);

            _locationId = _factory.Locations.Add(LocationName).Id;
            _flightId = _factory.Flights.Add(FlightNumber, Embarkation, Destination, AirlineName).Id;
            _aircraftId = _factory.Aircraft.Add(Registration, SerialNumber, YearOfManufacture, ModelName, ManufacturerName).Id;
            _sightingId = _factory.Sightings.Add(Altitude, SightingDate, _locationId, _flightId, _aircraftId).Id;
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Sighting sighting = _factory.Sightings.Get(a => a.Id == _sightingId);

            Assert.IsNotNull(sighting);
            Assert.AreEqual(_sightingId, sighting.Id);
            Assert.AreEqual(Altitude, sighting.Altitude);
            Assert.AreEqual(SightingDate, sighting.Date);

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
        public async Task AddAndGetAsyncTest()
        {
            long sightingId = (await _factory.Sightings.AddAsync(Altitude, AsyncSightingDate, _locationId, _flightId, _aircraftId)).Id;
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.Id == sightingId);

            Assert.IsNotNull(sighting);
            Assert.AreEqual(sightingId, sighting.Id);
            Assert.AreEqual(Altitude, sighting.Altitude);
            Assert.AreEqual(AsyncSightingDate, sighting.Date);

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
        public void GetMissingTest()
        {
            Sighting sighting = _factory.Sightings.Get(a => a.FlightId == 0);
            Assert.IsNull(sighting);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.List(null, 1, 100);
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual(_sightingId, sightings.First().Id);
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
        public void ListByAircraftTest()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByAircraft(Registration, 1, 100);
            Assert.AreEqual(1, sightings.Count());
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
        public void ListByAircraftWithNoSightingsTest()
        {
            _factory.Aircraft.Add("G-EZEH", "2184", 2004, ModelName, ManufacturerName);
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByAircraft("G-EZEH", 1, 100);
            Assert.AreEqual(0, sightings.Count());
        }

        [TestMethod]
        public void ListByMissingAircraftTest()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByAircraft("Missing", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public void ListByRouteTest()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByRoute(Embarkation, Destination, 1, 100);
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual(_sightingId, sightings.First().Id);
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
        public void ListByRouteWithNoSightingsTest()
        {
            _factory.Flights.Add("BA92", "YYZ", "LHR", "British Airways");
            IEnumerable <Sighting> sightings = _factory.Sightings.ListByRoute("YYZ", "LHR", 1, 100);
            Assert.AreEqual(0, sightings.Count());
        }

        [TestMethod]
        public void ListByMissingRoute()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByRoute("RMU", "MAN", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public void ListByAirlineTest()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByAirline(AirlineName, 1, 100);
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual(_sightingId, sightings.First().Id);
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
        public void ListByAirlineWithNoSightingsTest()
        {
            _factory.Airlines.Add("British Airways");
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByAirline("British Airways", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public void ListByMissingAirline()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByAirline("Missing", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public void ListByLocationTest()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByLocation(LocationName, 1, 100);
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public async Task ListByLocationAsyncTest()
        {
            IAsyncEnumerable<Sighting> matches = await _factory.Sightings
                                                               .ListByLocationAsync(LocationName, 1, 100);
            List<Sighting> sightings = await matches.ToListAsync();
            Assert.AreEqual(1, sightings.Count());
            Assert.AreEqual(_sightingId, sightings.First().Id);
        }

        [TestMethod]
        public void ListByLocationWithNoSightingsTest()
        {
            _factory.Airlines.Add("Gatwick Airport");
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByLocation("Gatwick Airport", 1, 100);
            Assert.IsNull(sightings);
        }

        [TestMethod]
        public void ListByMissingLocation()
        {
            IEnumerable<Sighting> sightings = _factory.Sightings.ListByLocation("Missing", 1, 100);
            Assert.IsNull(sightings);
        }
    }
}
