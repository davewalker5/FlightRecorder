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
        private const string AsyncCode = "LGW";
        private const string AsyncName = "Hatwick";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Airports.Add(Code, Name, Country);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Airports.Add(Code, Name, Country);
            Assert.AreEqual(1, _factory.Airports.List(null, 1, 100).Count());
            Assert.AreEqual(1, _factory.Airports.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Airport airport = _factory.Airports.Get(a => a.Code == Code);

            Assert.IsNotNull(airport);
            Assert.IsTrue(airport.Id > 0);
            Assert.AreEqual(Code, airport.Code);
            Assert.AreEqual(Name, airport.Name);

            Assert.IsNotNull(airport.Country);
            Assert.IsTrue(airport.Country.Id > 0);
            Assert.AreEqual(Country, airport.Country.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Airports.AddAsync(AsyncCode, AsyncName, Country);
            Airport airport = await _factory.Airports.GetAsync(a => a.Code == AsyncCode);

            Assert.IsNotNull(airport);
            Assert.IsTrue(airport.Id > 0);
            Assert.AreEqual(AsyncCode, airport.Code);
            Assert.AreEqual(AsyncName, airport.Name);

            Assert.IsNotNull(airport.Country);
            Assert.IsTrue(airport.Country.Id > 0);
            Assert.AreEqual(Country, airport.Country.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Airport airport = _factory.Airports.Get(a => a.Code == "Missing");
            Assert.IsNull(airport);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Airport> airports = _factory.Airports.List(null, 1, 100);
            Assert.AreEqual(1, airports.Count());
            Assert.AreEqual(Code, airports.First().Code);
            Assert.AreEqual(Name, airports.First().Name);
            Assert.AreEqual(Country, airports.First().Country.Name);
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
        public void FilteredListTest()
        {
            IEnumerable<Airport> airports = _factory.Airports.List(e => e.Code == Code, 1, 100);
            Assert.AreEqual(1, airports.Count());
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
        public void ListMissingTest()
        {
            IEnumerable<Airport> airports = _factory.Airports.List(e => e.Code == "Missing", 1, 100);
            Assert.AreEqual(0, airports.Count());
        }
    }
}
