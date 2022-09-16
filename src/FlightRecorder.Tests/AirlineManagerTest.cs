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
    public class AirlineManagerTest
    {
        private const string EntityName = "EasyJet";
        private const string AsyncEntityName = "Virgin Atlantic";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Airlines.Add(EntityName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Airlines.Add(EntityName);
            Assert.AreEqual(1, _factory.Airlines.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Airline entity = _factory.Airlines.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Airlines.AddAsync(AsyncEntityName);
            Airline entity = await _factory.Airlines
                                           .GetAsync(a => a.Name == AsyncEntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(AsyncEntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Airline entity = _factory.Airlines.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Airline> entities = _factory.Airlines.List(null, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Airline> entities = await _factory.Airlines
                                                   .ListAsync(null, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Airline> entities = _factory.Airlines.List(e => e.Name == EntityName, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Airline> entities = await _factory.Airlines
                                                   .ListAsync(e => e.Name == EntityName, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Airline> entities = _factory.Airlines.List(e => e.Name == "Missing", 1, 100);
            Assert.AreEqual(0, entities.Count());
        }
    }
}
