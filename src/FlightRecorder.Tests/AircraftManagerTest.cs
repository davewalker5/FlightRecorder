using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class AircraftManagerTest
    {
        private const string ModelName = "A380-861";
        private const string ManufacturerName = "Airbus";
        private const string AircraftAddress = "8960F9";
        private const string Registration = "A6-EUH";
        private const string SerialNumber = "220";
        private const string MissingDetailsRegistration = "G-EZTK";
        private const int YearOfManufacture = 2016;

        private FlightRecorderFactory _factory;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context, new MockFileLogger());
            long manufacturerId = Task.Run(() => _factory.Manufacturers.AddAsync(ManufacturerName)).Result.Id;
            long modelId = Task.Run(() => _factory.Models.AddAsync(ModelName, manufacturerId)).Result.Id;
            Task.Run(() => _factory.Aircraft.AddAsync(AircraftAddress, Registration, SerialNumber, YearOfManufacture, modelId)).Wait();
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            Aircraft aircraft = await _factory.Aircraft.GetAsync(a => a.Registration == Registration);

            Assert.IsNotNull(aircraft);
            Assert.IsTrue(aircraft.Id > 0);
            Assert.AreEqual(AircraftAddress, aircraft.Address);
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
        public async Task AddWithMissingDetailsAsyncTest()
        {
            var aircraft = await _factory.Aircraft.AddAsync(null, MissingDetailsRegistration, null, null, null);

            Assert.IsNotNull(aircraft);
            Assert.IsNull(aircraft.Address);
            Assert.AreEqual(MissingDetailsRegistration, aircraft.Registration);
            Assert.IsNull(aircraft.SerialNumber);
            Assert.IsNull(aircraft.Manufactured);
            Assert.IsNull(aircraft.Model);
        }

        [TestMethod]
        public async Task GetWithMissingDetailsAsyncTest()
        {
            await _factory.Aircraft.AddAsync(null, MissingDetailsRegistration, null, null, null);
            var aircraft = await _factory.Aircraft.GetAsync(x => x.Registration == MissingDetailsRegistration);

            Assert.IsNotNull(aircraft);
            Assert.IsNull(aircraft.Address);
            Assert.AreEqual(MissingDetailsRegistration, aircraft.Registration);
            Assert.IsNull(aircraft.SerialNumber);
            Assert.IsNull(aircraft.Manufactured);
            Assert.IsNull(aircraft.Model);
        }

        [TestMethod]
        public async Task UpdateAddressAsyncTest()
        {
            Aircraft existing = await _factory.Aircraft.GetAsync(a => a.Registration == Registration);
            Aircraft updated = await _factory.Aircraft.UpdateAsync(
                existing.Id,
                "  4ca213  ",
                existing.Registration,
                existing.SerialNumber,
                existing.Manufactured,
                existing.ModelId);

            Assert.AreEqual("4CA213", updated.Address);
        }

        [TestMethod]
        public async Task GetMissingAsyncTest()
        {
            Aircraft aircraft = await _factory.Aircraft.GetAsync(a => a.Registration == "Missing");
            Assert.IsNull(aircraft);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            List<Aircraft> aircraft = await _factory.Aircraft
                                                    .ListAsync(null, 1, 100)
                                                    .ToListAsync();
            Assert.AreEqual(1, aircraft.Count);
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
            Assert.AreEqual(1, aircraft.Count);
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
            Assert.AreEqual(1, aircraft.Count);
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
            Assert.AreEqual(1, aircraft.Count);
            Assert.AreEqual(Registration, aircraft.First().Registration);
            Assert.AreEqual(ModelName, aircraft.First().Model.Name);
            Assert.AreEqual(ManufacturerName, aircraft.First().Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task ListMissingAsyncTest()
        {
            List<Aircraft> aircraft = await _factory.Aircraft.ListAsync(e => e.Registration == "Missing", 1, 100).ToListAsync();
            Assert.AreEqual(0, aircraft.Count);
        }

        [TestMethod]
        public async Task ListByMissingModelAsyncTest()
        {
            IAsyncEnumerable<Aircraft> aircraft = await _factory.Aircraft.ListByModelAsync("Missing", 1, 100);
            Assert.IsNull(aircraft);
        }

        [TestMethod]
        public async Task ListByMissingManufacturerAsyncTest()
        {
            IAsyncEnumerable<Aircraft> aircraft = await _factory.Aircraft.ListByManufacturerAsync("Missing", 1, 100);
            Assert.IsNull(aircraft);
        }

        [TestMethod]
        public async Task ListByManufacturerWithNoModelsTest()
        {
            await _factory.Manufacturers.AddAsync("Boeing");
            IAsyncEnumerable<Aircraft> aircraft = await _factory.Aircraft.ListByManufacturerAsync("Boeing", 1, 100);
            Assert.IsNull(aircraft);
        }
    }
}
