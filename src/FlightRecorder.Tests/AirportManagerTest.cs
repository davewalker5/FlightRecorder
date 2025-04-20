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
    public class AirportManagerTest
    {
        private const string Code = "BHX";
        private const string Name = "Birmingham";
        private const string Country = "UK";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            Task.Run(() => _factory.Airports.AddAsync(Code, Name, Country)).Wait();
        }

        [TestMethod]
        public async Task AddDuplicateAsyncTest()
        {
            await _factory.Airports.AddAsync(Code, Name, Country);
            var airports = await _factory.Airports.ListAsync(null, 1, 100).ToListAsync();
            Assert.AreEqual(1, airports.Count);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Airport airport = await _factory.Airports.GetAsync(a => a.Code == Code);

            Assert.IsNotNull(airport);
            Assert.IsTrue(airport.Id > 0);
            Assert.AreEqual(Code, airport.Code);
            Assert.AreEqual(Name, airport.Name);

            Assert.IsNotNull(airport.Country);
            Assert.IsTrue(airport.Country.Id > 0);
            Assert.AreEqual(Country, airport.Country.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Airport airport = await _factory.Airports.GetAsync(a => a.Code == "Missing");
            Assert.IsNull(airport);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Airport> airports = await _factory.Airports
                                                 .ListAsync(null, 1, 100)
                                                 .ToListAsync();
            Assert.AreEqual(1, airports.Count);
            Assert.AreEqual(Code, airports.First().Code);
            Assert.AreEqual(Name, airports.First().Name);
            Assert.AreEqual(Country, airports.First().Country.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Airport> airports = await _factory.Airports
                                                 .ListAsync(e => e.Code == Code, 1, 100)
                                                 .ToListAsync();
            Assert.AreEqual(1, airports.Count);
            Assert.AreEqual(Code, airports.First().Code);
            Assert.AreEqual(Name, airports.First().Name);
            Assert.AreEqual(Country, airports.First().Country.Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Airport> airports = await _factory.Airports.ListAsync(e => e.Code == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, airports.Count);
        }
    }
}
