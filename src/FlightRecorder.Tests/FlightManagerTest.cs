﻿using System.Collections.Generic;
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
        private const string Embarkation = "LGW";
        private const string Destination = "RMU";
        private const string AirlineName = "EasyJet";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            Task.Run(() => _factory.Flights.AddAsync(FlightNumber, Embarkation, Destination, AirlineName)).Wait();
        }

        [TestMethod]
        public async Task AddDuplicateAsyncTest()
        {
            await _factory.Flights.AddAsync(FlightNumber, Embarkation, Destination, AirlineName);
            var flights = await _factory.Flights.ListAsync(null, 1, 100).ToListAsync();
            var airlines = await _factory.Airlines.ListAsync(null, 1, 100).ToListAsync();
            Assert.AreEqual(1, flights.Count);
            Assert.AreEqual(1, airlines.Count);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Flight flight = await _factory.Flights.GetAsync(a => a.Number == FlightNumber);

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
        public async Task GetMissingAsyncTest()
        {
            Flight flight = await _factory.Flights.GetAsync(a => a.Number == "Missing");
            Assert.IsNull(flight);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(null, 1, 100)
                                                 .ToListAsync();
            Assert.AreEqual(1, flights.Count);
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(e => e.Number == FlightNumber, 1, 100)
                                                 .ToListAsync();
            Assert.AreEqual(1, flights.Count);
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Flight> flights = await _factory.Flights.ListAsync(e => e.Number == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, flights.Count);
        }

        [TestMethod]
        public async Task ListByAirlineAsyncTest()
        {
            IAsyncEnumerable<Flight> matches = await _factory.Flights
                                                             .ListByAirlineAsync(AirlineName, 1, 100);
            List<Flight> flights = await matches.ToListAsync();
            Assert.AreEqual(1, flights.Count);
            Assert.AreEqual(FlightNumber, flights.First().Number);
            Assert.AreEqual(AirlineName, flights.First().Airline.Name);
        }

        [TestMethod]
        public async Task ListByMissingAirlineAsyncTest()
        {
            IAsyncEnumerable<Flight> flights = await _factory.Flights.ListByAirlineAsync("Missing", 1, 100);
            Assert.IsNull(flights);
        }
    }
}
