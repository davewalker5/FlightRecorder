using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace FlightRecorder.Tests
{
    /// <summary>
    /// These tests can't test authentication/authorisation at the service end, the lookup of data at the
    /// service end or network transport. They're design to test the downstream logic once a response has
    /// been received
    /// </summary>
    [TestClass]
    public class FlightLookupTest
    {
        private const string EndpointBaseUrl = "https://aerodatabox.p.rapidapi.com/flights/number/";
        private const string FlightNumber = "LS803";
        private const string MalformedResponse = "{\"departure\": {\"airport\": {\"iata\": \"MAN\"}}}";
        private const string Response = "[{\"departure\": {\"airport\": {\"iata\": \"MAN\"}},\"arrival\": {\"airport\": {\"iata\": \"BCN\"}},\"airline\": {\"name\": \"Jet2\"}}]";

        private MockHttpClient _client;
        private IFlightsApi _api;

        [TestInitialize]
        public void Initialise()
        {
            // Create the logger, mock client and API wrappers
            var logger = new MockFileLogger();
            _client = new MockHttpClient();
            _api = new AeroDataBoxFlightsApi(logger, _client, EndpointBaseUrl, "");
        }

        [TestMethod]
        public void FlightNotFoundByNumberTest()
        {
            _client!.AddResponse("");
            var properties = Task.Run(() => _api.LookupFlightByNumber("Flight Doesn't Exist")).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void FlightNotFoundByNumberAndDateTest()
        {
            _client!.AddResponse("");
            var properties = Task.Run(() => _api.LookupFlightByNumberAndDate("Flight Doesn't Exist", DateTime.Now)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void NetworkErrorTest()
        {
            _client!.AddResponse(null);
            var properties = Task.Run(() => _api.LookupFlightByNumber(FlightNumber)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void MalformedResponseTest()
        {
            _client!.AddResponse(MalformedResponse);
            var properties = Task.Run(() => _api.LookupFlightByNumber(FlightNumber)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void LookupFlightByNumberTest()
        {
            _client!.AddResponse(Response);
            var properties = Task.Run(() => _api.LookupFlightByNumber(FlightNumber)).Result;
            Assert.IsNotNull(properties);

            var embarkation = properties[ApiPropertyType.DepartureAirportIATA];
            var destination = properties[ApiPropertyType.DestinationAirportIATA];
            var airline = properties[ApiPropertyType.AirlineName];

            Assert.AreEqual("MAN", embarkation);
            Assert.AreEqual("BCN", destination);
            Assert.AreEqual("Jet2", airline);
        }

        [TestMethod]
        public void LookupFlightByNumberAndDateTest()
        {
            _client!.AddResponse(Response);
            var properties = Task.Run(() => _api.LookupFlightByNumberAndDate(FlightNumber, DateTime.Now)).Result;
            Assert.IsNotNull(properties);

            var embarkation = properties[ApiPropertyType.DepartureAirportIATA];
            var destination = properties[ApiPropertyType.DestinationAirportIATA];
            var airline = properties[ApiPropertyType.AirlineName];

            Assert.AreEqual("MAN", embarkation);
            Assert.AreEqual("BCN", destination);
            Assert.AreEqual("Jet2", airline);
        }
    }
}
