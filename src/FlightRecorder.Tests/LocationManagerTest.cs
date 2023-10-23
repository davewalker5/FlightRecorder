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

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            Task.Run(() => _factory.Locations.AddAsync(EntityName)).Wait();
        }

        [TestMethod]
        public async Task AddDuplicateAsyncTest()
        {
            await _factory.Locations.AddAsync(EntityName);
            var locations = await _factory.Locations.ListAsync(null, 1, 100).ToListAsync();
            Assert.AreEqual(1, locations.Count());
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Location entity = await _factory.Locations.GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Location entity = await _factory.Locations.GetAsync(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Location> entities = await _factory.Locations
                                                    .ListAsync(null, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Location> entities = await _factory.Locations
                                                    .ListAsync(e => e.Name == EntityName, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            IEnumerable<Location> entities = await _factory.Locations.ListAsync(e => e.Name == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, entities.Count());
        }
    }
}
