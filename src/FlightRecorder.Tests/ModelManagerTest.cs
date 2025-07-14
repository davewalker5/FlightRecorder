using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class ModelManagerTest
    {
        private const string ModelName = "A380-861";
        private const string ManufacturerName = "Airbus";

        private FlightRecorderFactory _factory;
        private long _manufacturerId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context, new MockFileLogger());
            _manufacturerId = Task.Run(() => _factory.Manufacturers.AddAsync(ManufacturerName)).Result.Id;
            Console.WriteLine(_manufacturerId);
            Task.Run(() => _factory.Models.AddAsync(ModelName, _manufacturerId)).Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(ModelExistsException))]
        public async Task CannotAddDuplicateAsyncTest()
            => await _factory.Models.AddAsync(ModelName, _manufacturerId);

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Model model = await _factory.Models.GetAsync(a => a.Name == ModelName);

            Assert.IsNotNull(model);
            Assert.IsTrue(model.Id > 0);
            Assert.AreEqual(ModelName, model.Name);

            Assert.IsNotNull(model.Manufacturer);
            Assert.IsTrue(model.Manufacturer.Id > 0);
            Assert.AreEqual(ManufacturerName, model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Model model = await _factory.Models.GetAsync(a => a.Name == "Missing");
            Assert.IsNull(model);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Model> models = await _factory.Models
                                               .ListAsync(null, 1, 100)
                                               .ToListAsync();
            Assert.AreEqual(1, models.Count);
            Assert.AreEqual(ModelName, models.First().Name);
            Assert.AreEqual(ManufacturerName, models.First().Manufacturer.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Model> models = await _factory.Models
                                               .ListAsync(e => e.Name == ModelName, 1, 100)
                                               .ToListAsync();
            Assert.AreEqual(1, models.Count);
            Assert.AreEqual(ModelName, models.First().Name);
            Assert.AreEqual(ManufacturerName, models.First().Manufacturer.Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Model> models = await _factory.Models.ListAsync(e => e.Name == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, models.Count);
        }

        [TestMethod]
        public async Task ListByManufacturerAsyncTest()
        {
            List<Model> models = await _factory.Models
                                               .ListByManufacturerAsync(ManufacturerName, 1, 100)
                                               .ToListAsync();
            Assert.AreEqual(1, models.Count);
            Assert.AreEqual(ModelName, models.First().Name);
            Assert.AreEqual(ManufacturerName, models.First().Manufacturer.Name);
        }

        [TestMethod]
        public async Task ListByMissingManufacturerAsyncTest()
        {
            IEnumerable<Model> models = await _factory.Models
                                                      .ListByManufacturerAsync("Missing", 1, 100)
                                                      .ToListAsync();
            Assert.IsFalse(models.Any());
        }
    }
}
