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
        private const string AsyncEntityName = "Boeing";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Manufacturers.Add(EntityName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Manufacturers.Add(EntityName);
            Assert.AreEqual(1, _factory.Manufacturers.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Manufacturer entity = _factory.Manufacturers
                                          .Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Manufacturers.AddAsync(AsyncEntityName);
            Manufacturer entity = await _factory.Manufacturers
                                                .GetAsync(m => m.Name == AsyncEntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(AsyncEntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Manufacturer entity = _factory.Manufacturers
                                          .Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Manufacturer> entities = _factory.Manufacturers
                                                         .List(null, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
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
        public void FilteredListTest()
        {
            IEnumerable<Manufacturer> entities = _factory.Manufacturers
                                                         .List(e => e.Name == EntityName, 1, 100);
            Assert.AreEqual(1, entities.Count());
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
        public void ListMissingTest()
        {
            IEnumerable<Manufacturer> entities = _factory.Manufacturers
                                                         .List(e => e.Name == "Missing", 1, 100);
            Assert.AreEqual(0, entities.Count());
        }
    }
}
