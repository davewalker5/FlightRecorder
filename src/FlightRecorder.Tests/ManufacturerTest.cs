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
    public class ManufacturerManagerTest
    {
        private const string EntityName = "Airbus";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            Task.Run(() => _factory.Manufacturers.AddAsync(EntityName)).Wait();
        }

        [TestMethod]
        public async Task AddDuplicateAsyncTest()
        {
            await _factory.Manufacturers.AddAsync(EntityName);
            var manufacturers = await _factory.Manufacturers.ListAsync(null, 1, 100).ToListAsync();
            Assert.AreEqual(1, manufacturers.Count);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Manufacturer entity = await _factory.Manufacturers
                                                .GetAsync(m => m.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Manufacturer entity = await _factory.Manufacturers
                                                .GetAsync(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Manufacturer> entities = await _factory.Manufacturers
                                                        .ListAsync(null, 1, 100)
                                                        .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Manufacturer> entities = await _factory.Manufacturers
                                                        .ListAsync(e => e.Name == EntityName, 1, 100)
                                                        .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Manufacturer> entities = await _factory.Manufacturers
                                                               .ListAsync(e => e.Name == "Missing", 1, 100)
                                                               .ToListAsync();
            Assert.AreEqual(0, entities.Count);
        }
    }
}
