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

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            Task.Run(() => _factory.Countries.AddAsync(EntityName)).Wait();
        }

        [TestMethod]
        public async Task AddDuplicateAsyncTest()
        {
            var countries = await _factory.Countries.ListAsync(null, 1, 100).ToListAsync();
            Assert.AreEqual(1, countries.Count);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Countries.AddAsync(EntityName);
            Country entity = await _factory.Countries
                                           .GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Country entity = await _factory.Countries.GetAsync(a => a.Name == "Missing");
            Assert.IsNull(entity);
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
        public async Task FilteredListAsyncTest()
        {
            List<Country> entities = await _factory.Countries
                                                   .ListAsync(e => e.Name == EntityName, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Country> entities = await _factory.Countries.ListAsync(e => e.Name == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, entities.Count);
        }
    }
}
