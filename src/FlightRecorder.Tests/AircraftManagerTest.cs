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
    public class AircraftManagerTest
    {
        private const string ModelName = "A380-861";
        private const string ManufacturerName = "Airbus";
        private const string Registration = "A6-EUH";
        private const string AsyncRegistration = "A6-EDM";
        private const string SerialNumber = "220";
        private const string AsyncSerialNumber = "42";
        private const int YearOfManufacture = 2016;
        private const int AsyncYearOfManufacture = 2010;

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = new FlightRecorderDbContextFactory().CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _factory.Aircraft.Add(Registration, SerialNumber, YearOfManufacture, ModelName, ManufacturerName);
        }

        [TestMethod]
        public void AddAndGetTest()
        {
            Aircraft aircraft = _factory.Aircraft.Get(a => a.Registration == Registration);

            Assert.IsNotNull(aircraft);
            Assert.IsTrue(aircraft.Id > 0);
            Assert.AreEqual(Registration, aircraft.Registration);
            Assert.AreEqual(SerialNumber, aircraft.SerialNumber);
            Assert.AreEqual(YearOfManufacture, aircraft.Manufactured);

            Assert.IsNotNull(aircraft.Model);
            Assert.IsTrue(aircraft.Model.Id > 0);
            Assert.AreEqual(ModelName, aircraft.Model.Name);

            Assert.IsNotNull(aircraft.Model.Manufacturer);
            Assert.IsTrue(aircraft.Model.Manufacturer.Id > 0);
            Assert.AreEqual(ManufacturerName, aircraft.Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            await _factory.Aircraft.AddAsync(AsyncRegistration, AsyncSerialNumber, AsyncYearOfManufacture, ModelName, ManufacturerName);
            Aircraft aircraft = await _factory.Aircraft.GetAsync(a => a.Registration == AsyncRegistration);

            Assert.IsNotNull(aircraft);
            Assert.IsTrue(aircraft.Id > 0);
            Assert.AreEqual(AsyncRegistration, aircraft.Registration);
            Assert.AreEqual(AsyncSerialNumber, aircraft.SerialNumber);
            Assert.AreEqual(AsyncYearOfManufacture, aircraft.Manufactured);

            Assert.IsNotNull(aircraft.Model);
            Assert.IsTrue(aircraft.Model.Id > 0);
            Assert.AreEqual(ModelName, aircraft.Model.Name);

            Assert.IsNotNull(aircraft.Model.Manufacturer);
            Assert.IsTrue(aircraft.Model.Manufacturer.Id > 0);
            Assert.AreEqual(ManufacturerName, aircraft.Model.Manufacturer.Name);
        }

        [TestMethod]
        public void GetMissingTest()
        {
            Aircraft aircraft = _factory.Aircraft.Get(a => a.Registration == "Missing");
            Assert.IsNull(aircraft);
        }

        [TestMethod]
        public void ListAllTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.List(null, 1, 100);
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Aircraft> aircraft = await _factory.Aircraft
                                                    .ListAsync(null, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public void FilteredListTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.List(e => e.Registration == Registration, 1, 100);
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task FilteredListAsyncTest()
        {
            List<Aircraft> aircraft = await _factory.Aircraft
                                                    .ListAsync(e => e.Registration == Registration, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public void FilterByModelTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.ListByModel(ModelName, 1, 100);
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task FilterByModelAsyncTest()
        {
            IAsyncEnumerable<Aircraft> matches = await _factory.Aircraft
                                                               .ListByModelAsync(ModelName, 1, 100);
            List<Aircraft> aircraft = await matches.ToListAsync();
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public void FilterByManufacturerTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.ListByManufacturer(ManufacturerName, 1, 100);
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task FilterByManufacturerAsyncTest()
        {
            IAsyncEnumerable<Aircraft> matches = await _factory.Aircraft
                                                               .ListByManufacturerAsync(ManufacturerName, 1, 100);
            List<Aircraft> aircraft = await matches.ToListAsync();
            Assert.AreEqual(1, aircraft.Count());
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public void ListMissingTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.List(e => e.Registration == "Missing", 1, 100);
            Assert.AreEqual(0, aircraft.Count());
        }

        [TestMethod]
        public void ListByMissingModelTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.ListByModel("Missing", 1, 100);
            Assert.IsNull(aircraft);
        }

        [TestMethod]
        public void ListByMissingManufacturerTest()
        {
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.ListByManufacturer("Missing", 1, 100);
            Assert.IsNull(aircraft);
        }

        [TestMethod]
        public void ListByManufacturerWithNoModelsTest()
        {
            _factory.Manufacturers.Add("Boeing");
            IEnumerable<Aircraft> aircraft = _factory.Aircraft.ListByManufacturer("Boeing", 1, 100);
            Assert.IsNull(aircraft);
        }
    }
}
