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
    public class FlightManagerTest
    {
        private const string FlightNumber = "U28551";
        private const string AsyncFlightNumber = "U28550";
        private const string Embarkation = "LGW";
        private const string AsyncEmbarkation = "RMU";
        private const string Destination = "RMU";
        private const string AsyncDestination = "LGW";
        private const string AirlineName = "EasyJet";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Flights.Add(FlightNumber, Embarkation, Destination, AirlineName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Flights.Add(FlightNumber, Embarkation, Destination, AirlineName);
            Assert.AreEqual(1, _factory.Flights.List(null, 1, 100).Count());
            Assert.AreEqual(1, _factory.Airlines.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Flight flight = _factory.Flights.Get(a => a.Number == FlightNumber);

            Assert.IsNotNull(flight);
            Assert.IsTrue(flight.Id > 0);
            Assert.AreEqual(FlightNumber, flight.Number);
            Assert.AreEqual(Embarkation, flight.Embarkation);
            Assert.AreEqual(Destination, flight.Destination);

            Assert.IsNotNull(flight.Airline);
            Assert.IsTrue(flight.Airline.Id > 0);
            Assert.AreEqual(AirlineName, flight.Airline.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Flights.AddAsync(AsyncFlightNumber, AsyncEmbarkation, AsyncDestination, AirlineName);
            Flight flight = await _factory.Flights.GetAsync(a => a.Number == AsyncFlightNumber);

            Assert.IsNotNull(flight);
            Assert.IsTrue(flight.Id > 0);
            Assert.AreEqual(AsyncFlightNumber, flight.Number);
            Assert.AreEqual(AsyncEmbarkation, flight.Embarkation);
            Assert.AreEqual(AsyncDestination, flight.Destination);

            Assert.IsNotNull(flight.Airline);
            Assert.IsTrue(flight.Airline.Id > 0);
            Assert.AreEqual(AirlineName, flight.Airline.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Flight flight = _factory.Flights.Get(a => a.Number == "Missing");
            Assert.IsNull(flight);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.List(null, 1, 100);
            Assert.AreEqual(1, flights.Count());
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(null, 1, 100)
                                                 .ToListAsync();
            Assert.AreEqual(1, flights.Count());
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.List(e => e.Number == FlightNumber, 1, 100);
            Assert.AreEqual(1, flights.Count());
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(e => e.Number == FlightNumber, 1, 100)
                                                 .ToListAsync();
            Assert.AreEqual(1, flights.Count());
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.List(e => e.Number == "Missing", 1, 100);
            Assert.AreEqual(0, flights.Count());
        }

        [TestMethod]
        public void ListByAirlineTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.ListByAirline(AirlineName, 1, 100);
            Assert.AreEqual(1, flights.Count());
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public async Task ListByAirlineAsyncTest()
        {
            IAsyncEnumerable<Flight> matches = await _factory.Flights
                                                             .ListByAirlineAsync(AirlineName, 1, 100);
            List<Flight> flights = await matches.ToListAsync();
            Assert.AreEqual(1, flights.Count());
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public void ListByMissingAirlineTest()
        {
            IEnumerable<Flight> flights = _factory.Flights.ListByAirline("Missing", 1, 100);
            Assert.IsNull(flights);
        }
    }
}
