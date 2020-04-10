using System.Collections.Generic;
using System.Linq;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class ModelManagerTest
    {
        private const string ModelName = "A380-861";
        private const string ManufacturerName = "Airbus";

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Models.Add(ModelName, ManufacturerName);
        }

        [TestMethod]
        public void AddDuplicateTest()
        {
            _factory.Models.Add(ModelName, ManufacturerName);
            Assert.AreEqual(1, _factory.Models.List().Count());
            Assert.AreEqual(1, _factory.Manufacturers.List().Count());
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Model model = _factory.Models.Get(a => a.Name == ModelName);

            Assert.IsNotNull(model);
            Assert.IsTrue(model.Id > 0);
            Assert.AreEqual(ModelName, model.Name);

            Assert.IsNotNull(model.Manufacturer);
            Assert.IsTrue(model.Manufacturer.Id > 0);
            Assert.AreEqual(ManufacturerName, model.Manufacturer.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Model model = _factory.Models.Get(a => a.Name == "Missing");
            Assert.IsNull(model);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Model> models = _factory.Models.List();
            Assert.AreEqual(1, models.Count());
            Assert.AreEqual(ModelName, models.First().Name);
            Assert.AreEqual(ManufacturerName, models.First().Manufacturer.Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Model> models = _factory.Models.List(e => e.Name == ModelName);
            Assert.AreEqual(1, models.Count());
            Assert.AreEqual(ModelName, models.First().Name);
            Assert.AreEqual(ManufacturerName, models.First().Manufacturer.Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Model> models = _factory.Models.List(e => e.Name == "Missing");
            Assert.AreEqual(0, models.Count());
        }

        [TestMethod]
        public void ListByManufacturerTest()
        {
            IEnumerable<Model> models = _factory.Models.ListByManufacturer(ManufacturerName);
            Assert.AreEqual(1, models.Count());
            Assert.AreEqual(ModelName, models.First().Name);
            Assert.AreEqual(ManufacturerName, models.First().Manufacturer.Name);
        }

        [TestMethod]
        public void ListByMissingManufacturerTest()
        {
            IEnumerable<Model> models = _factory.Models.ListByManufacturer("Missing");
            Assert.IsFalse(models.Any());
        }
    }
}
