using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class DataExchangeTests
    {
        private const string LocationName = "Murcia Corvera International Airport";

        private const string FlightNumber = "U28551";
        private const string Embarkation = "LGW";
        private const string Destination = "RMU";
        private const string AirlineName = "EasyJet";

        private const string ModelName = "A319-111";
        private const string ManufacturerName = "Airbus";
        private const string Registration = "G-EZFY";
        private const string SerialNumber = "4418";
        private const string Age = "9";

        private const long Altitude = 930;
        private readonly DateTime SightingDate = new DateTime(2019, 9, 22);

        private FlightRecorderFactory _factory;
        private long _sightingId;

        [TestInitialize]
        public void TestInitialize()
        {
            FlightRecorderDbContext context = FlightRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new FlightRecorderFactory(context);
            _sightingId = Task.Run(() => _factory.Sightings.AddAsync(new FlattenedSighting
            {
                FlightNumber = FlightNumber,
                Airline = AirlineName,
                Registration = Registration,
                SerialNumber = SerialNumber,
                Manufacturer = ManufacturerName,
                Model = ModelName,
                Age = Age,
                Embarkation = Embarkation,
                Destination = Destination,
                Altitude = Altitude,
                Date = SightingDate,
                Location = LocationName
            })).Result.Id;
        }

        [TestMethod]
        public async Task AddAndGetTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.Id == _sightingId);

            Assert.IsNotNull(sighting);
            Assert.AreEqual(Altitude, sighting.Altitude);
            Assert.AreEqual(SightingDate, sighting.Date);

            Assert.IsNotNull(sighting.Location);
            Assert.AreEqual(LocationName, sighting.Location.Name);

            Assert.IsNotNull(sighting.Flight);
            Assert.AreEqual(FlightNumber, sighting.Flight.Number);
            Assert.AreEqual(Embarkation, sighting.Flight.Embarkation);
            Assert.AreEqual(Destination, sighting.Flight.Destination);

            Assert.IsNotNull(sighting.Flight.Airline);
            Assert.AreEqual(AirlineName, sighting.Flight.Airline.Name);

            Assert.IsNotNull(sighting.Aircraft);
            Assert.AreEqual(Registration, sighting.Aircraft.Registration);
            Assert.AreEqual(SerialNumber, sighting.Aircraft.SerialNumber);
            Assert.AreEqual(DateTime.Now.Year - long.Parse(Age), sighting.Aircraft.Manufactured);

            Assert.IsNotNull(sighting.Aircraft.Model);
            Assert.AreEqual(ModelName, sighting.Aircraft.Model.Name);

            Assert.IsNotNull(sighting.Aircraft.Model.Manufacturer);
            Assert.AreEqual(ManufacturerName, sighting.Aircraft.Model.Manufacturer.Name);
        }

        [TestMethod]
        public async Task FlattenCollectionTest()
        {
            IEnumerable<Sighting> sightings = await _factory.Sightings.ListAsync(null, 1, 100).ToListAsync();
            IEnumerable<FlattenedSighting> flattened = sightings.Flatten();
            Assert.AreEqual(1, flattened.Count());
            Assert.AreEqual(FlightNumber, flattened.First().FlightNumber);
            Assert.AreEqual(AirlineName, flattened.First().Airline);
            Assert.AreEqual(Registration, flattened.First().Registration);
            Assert.AreEqual(SerialNumber, flattened.First().SerialNumber);
            Assert.AreEqual(ManufacturerName, flattened.First().Manufacturer);
            Assert.AreEqual(ModelName, flattened.First().Model);
            Assert.AreEqual(Age, flattened.First().Age);
            Assert.AreEqual(Embarkation, flattened.First().Embarkation);
            Assert.AreEqual(Destination, flattened.First().Destination);
            Assert.AreEqual(Altitude, flattened.First().Altitude);
            Assert.AreEqual(SightingDate, flattened.First().Date);
            Assert.AreEqual(LocationName, flattened.First().Location);
        }

        [TestMethod]
        public async Task FlattenedToCsvTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.Id == _sightingId);
            FlattenedSighting flattened = sighting.Flatten();
            string csvRecord = flattened.ToCsv();
            Regex regex = new Regex(FlattenedSighting.CsvRecordPattern);
            bool matches = regex.Matches(csvRecord).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public async Task InflateFromCsvTest()
        {
            Sighting sighting = await _factory.Sightings.GetAsync(a => a.Id == _sightingId);
            FlattenedSighting flattened = sighting.Flatten();
            string csvRecord = flattened.ToCsv();
            FlattenedSighting inflated = FlattenedSighting.FromCsv(csvRecord);
            Assert.AreEqual(flattened.FlightNumber, inflated.FlightNumber);
            Assert.AreEqual(flattened.Airline, inflated.Airline);
            Assert.AreEqual(flattened.Registration, inflated.Registration);
            Assert.AreEqual(flattened.SerialNumber, inflated.SerialNumber);
            Assert.AreEqual(flattened.Manufacturer, inflated.Manufacturer);
            Assert.AreEqual(flattened.Model, inflated.Model);
            Assert.AreEqual(flattened.Age, inflated.Age);
            Assert.AreEqual(flattened.Embarkation, inflated.Embarkation);
            Assert.AreEqual(flattened.Destination, inflated.Destination);
            Assert.AreEqual(flattened.Altitude, inflated.Altitude);
            Assert.AreEqual(flattened.Date, inflated.Date);
            Assert.AreEqual(flattened.Location, inflated.Location);
        }
    }
}
