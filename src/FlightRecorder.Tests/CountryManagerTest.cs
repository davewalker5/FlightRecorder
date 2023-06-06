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
    public class CountryManagerTest
    {
        private const string EntityName = "UK";
        private const string AsyncEntityName = "USA";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Countries.Add(EntityName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Countries.Add(EntityName);
            Assert.AreEqual(1, _factory.Countries.List(null, 1, 100).Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Country entity = _factory.Countries.Get(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Countries.AddAsync(AsyncEntityName);
            Country entity = await _factory.Countries
                                           .GetAsync(a => a.Name == AsyncEntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(AsyncEntityName, entity.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Country entity = _factory.Countries.Get(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Country> entities = _factory.Countries.List(null, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Country> entities = await _factory.Countries
                                                   .ListAsync(null, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Country> entities = _factory.Countries.List(e => e.Name == EntityName, 1, 100);
            Assert.AreEqual(1, entities.Count());
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Country> entities = await _factory.Countries
                                                   .ListAsync(e => e.Name == EntityName, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Country> entities = _factory.Countries.List(e => e.Name == "Missing", 1, 100);
            Assert.AreEqual(0, entities.Count());
        }
    }
}
