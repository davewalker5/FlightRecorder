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
    public class LocationManagerTest
    {
        private const string EntityName = "Gatwick Airport";
        private const string AsyncEntityName = "Heathrow Airport";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Locations.Add(EntityName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Locations.Add(EntityName);
            Assert.AreEqual(1, _factory.Locations.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Location entity = _factory.Locations.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Locations.AddAsync(AsyncEntityName);
            Location entity = await _factory.Locations.GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Location entity = _factory.Locations.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Location> entities = _factory.Locations.List(null, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Location> entities = await _factory.Locations
                                                    .ListAsync(null, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Location> entities = _factory.Locations.List(e => e.Name == EntityName, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Location> entities = await _factory.Locations
                                                    .ListAsync(e => e.Name == EntityName, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Location> entities = _factory.Locations.List(e => e.Name == "Missing", 1, 100);
            Assert.AreEqual(0, entities.Count());
        }
    }
}
