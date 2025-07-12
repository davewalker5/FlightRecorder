using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class AirlineManagerTest
    {
        private const string EntityName = "EasyJet";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context, new MockFileLogger());
            Task.Run(() => _factory.Airlines.AddAsync(EntityName)).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AirlineExistsException))]
        public async Task CannotAddDuplicateAsyncTest()
            => await _factory.Airlines.AddAsync(EntityName);

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Airline entity = await _factory.Airlines
                                           .GetAsync(a => a.Name == EntityName);
            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id > 0);
            Assert.AreEqual(EntityName, entity.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Airline entity = await _factory.Airlines.GetAsync(a => a.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Airline> entities = await _factory.Airlines
                                                   .ListAsync(null, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Airline> entities = await _factory.Airlines
                                                   .ListAsync(e => e.Name == EntityName, 1, 100)
                                                   .ToListAsync();
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(EntityName, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Airline> entities = await _factory.Airlines.ListAsync(e => e.Name == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, entities.Count);
        }
    }
}
