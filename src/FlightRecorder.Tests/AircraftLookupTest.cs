using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightRecorder.Tests
{
    /// <summary>
    /// These tests can't test authentication/authorisation at the service end, the lookup of data at the
    /// service end or network transport. They're design to test the downstream logic once a response has
    /// been received
    /// </summary>
    [TestClass]
    public class AircraftLookupTest
    {
        private const string EndpointBaseUrl = "https://aerodatabox.p.rapidapi.com/aircrafts/";
        private const string Registration = "EI-DEA";
        private const string SerialNumber = "2191";
        private const string ICAOAddress = "4CA213";
        private const string Model = "A320";
        private const string ModelCode = "320-214";
        private const string RegistrationDate = "2004-05-04";
        private const string TypeName = "Airbus A320";
        private const string ProductionLine = "Airbus A320";
        private const string Manufacturer = "Airbus";
        private const string MalformedResponse = "[{}]";
        private const string MalformedAgeResponse = "{ \"reg\": \"EI-DEA\", \"serial\": \"2191\", \"hexIcao\": \"4CA213\", \"model\": \"A320\", \"modelCode\": \"320-214\", \"registrationDate\": \"This is not a date\", \"typeName\": \"Airbus A320\", \"productionLine\": \"Airbus A320\" }";
        private const string Response = "{ \"reg\": \"EI-DEA\", \"serial\": \"2191\", \"hexIcao\": \"4CA213\", \"model\": \"A320\", \"modelCode\": \"320-214\", \"registrationDate\": \"2004-05-04\", \"typeName\": \"Airbus A320\", \"productionLine\": \"Airbus A320\" }";

        private MockExternalApiHttpClient _client;
        private IAircraftApi _api;

        [TestInitialize]
        public void Initialise()
        {
            // Create the logger, mock client and API wrappers
            var logger = new MockFileLogger();
            _client = new MockExternalApiHttpClient();
            _api = new AeroDataBoxAircraftApi(logger, _client, EndpointBaseUrl, "",
                new FixedTimeProvider(new DateTimeOffset(2026, 11, 16, 12, 0, 0, TimeSpan.Zero)));
        }

        [TestMethod]
        public void AircraftNotFoundByRegistrationTest()
        {
            _client!.AddResponse("");
            var properties = Task.Run(() => _api.LookupAircraftByRegistration("Aircraft Doesn't Exist")).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void AircraftNotFoundByICAOAddressTest()
        {
            _client!.AddResponse("");
            var properties = Task.Run(() => _api.LookupAircraftByICAOAddress("Aircraft Doesn't Exist")).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void NetworkErrorTest()
        {
            _client!.AddResponse(null);
            var properties = Task.Run(() => _api.LookupAircraftByRegistration(Registration)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void MalformedResponseTest()
        {
            _client!.AddResponse(MalformedResponse);
            var properties = Task.Run(() => _api.LookupAircraftByRegistration(Registration)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void MalformedAgeResponseTest()
        {
            _client!.AddResponse(MalformedAgeResponse);
            var properties = Task.Run(() => _api.LookupAircraftByRegistration(Registration)).Result;

            Assert.IsNotNull(properties);
            Assert.AreEqual(Registration, properties[ApiPropertyType.AircraftRegistration]);
            Assert.AreEqual(SerialNumber, properties[ApiPropertyType.AircraftSerialNumber]);
            Assert.AreEqual(ICAOAddress, properties[ApiPropertyType.AircraftICAOAddress]);
            Assert.AreEqual(Model, properties[ApiPropertyType.AircraftModel]);
            Assert.AreEqual(ModelCode, properties[ApiPropertyType.AircraftModelCode]);
            Assert.AreEqual(TypeName, properties[ApiPropertyType.AircraftType]);
            Assert.AreEqual(ProductionLine, properties[ApiPropertyType.AircraftProductionLine]);
            Assert.AreEqual("", properties[ApiPropertyType.AircraftAge]);
            Assert.AreEqual(Manufacturer, properties[ApiPropertyType.ManufacturerName]);
        }

        [TestMethod]
        [DataRow(5, 3, "22")]
        [DataRow(11, 2, "22")]
        [DataRow(11, 3, "23")]
        [DataRow(11, 16, "23")]
        public void LookupAircraftByRegistrationTest(int month, int day, string expectedAge)
        {
            _api = new AeroDataBoxAircraftApi(new MockFileLogger(), _client, EndpointBaseUrl, "",
                new FixedTimeProvider(new DateTimeOffset(2026, month, day, 12, 0, 0, TimeSpan.Zero)));
            _client!.AddResponse(Response);
            var properties = Task.Run(() => _api.LookupAircraftByRegistration(Registration)).Result;

            Assert.IsNotNull(properties);
            Assert.AreEqual(Registration, properties[ApiPropertyType.AircraftRegistration]);
            Assert.AreEqual(SerialNumber, properties[ApiPropertyType.AircraftSerialNumber]);
            Assert.AreEqual(ICAOAddress, properties[ApiPropertyType.AircraftICAOAddress]);
            Assert.AreEqual(Model, properties[ApiPropertyType.AircraftModel]);
            Assert.AreEqual(ModelCode, properties[ApiPropertyType.AircraftModelCode]);
            Assert.AreEqual(RegistrationDate, properties[ApiPropertyType.AircraftRegistrationDate]);
            Assert.AreEqual(TypeName, properties[ApiPropertyType.AircraftType]);
            Assert.AreEqual(ProductionLine, properties[ApiPropertyType.AircraftProductionLine]);
            Assert.AreEqual(expectedAge, properties[ApiPropertyType.AircraftAge]);
            Assert.AreEqual(Manufacturer, properties[ApiPropertyType.ManufacturerName]);
        }

        [TestMethod]
        [DataRow(5, 3, "22")]
        [DataRow(11, 2, "22")]
        [DataRow(11, 3, "23")]
        [DataRow(11, 16, "23")]
        public void LookupAircraftByICAOAddressTest(int month, int day, string expectedAge)
        {
            _api = new AeroDataBoxAircraftApi(new MockFileLogger(), _client, EndpointBaseUrl, "",
                new FixedTimeProvider(new DateTimeOffset(2026, month, day, 12, 0, 0, TimeSpan.Zero)));
            _client!.AddResponse(Response);
            var properties = Task.Run(() => _api.LookupAircraftByICAOAddress(ICAOAddress)).Result;

            Assert.IsNotNull(properties);
            Assert.AreEqual(Registration, properties[ApiPropertyType.AircraftRegistration]);
            Assert.AreEqual(SerialNumber, properties[ApiPropertyType.AircraftSerialNumber]);
            Assert.AreEqual(ICAOAddress, properties[ApiPropertyType.AircraftICAOAddress]);
            Assert.AreEqual(Model, properties[ApiPropertyType.AircraftModel]);
            Assert.AreEqual(ModelCode, properties[ApiPropertyType.AircraftModelCode]);
            Assert.AreEqual(RegistrationDate, properties[ApiPropertyType.AircraftRegistrationDate]);
            Assert.AreEqual(TypeName, properties[ApiPropertyType.AircraftType]);
            Assert.AreEqual(ProductionLine, properties[ApiPropertyType.AircraftProductionLine]);
            Assert.AreEqual(expectedAge, properties[ApiPropertyType.AircraftAge]);
            Assert.AreEqual(Manufacturer, properties[ApiPropertyType.ManufacturerName]);
        }

        // Keep lookup tests independent of the machine's date and time zone.
        private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => now;
            public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        }
    }
}
