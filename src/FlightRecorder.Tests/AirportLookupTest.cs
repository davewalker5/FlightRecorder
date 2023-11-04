using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace FlightRecorder.Tests
{
    /// <summary>
    /// These tests can't test authentication/authorisation at the service end, the lookup of data at the
    /// service end or network transport. They're design to test the downstream logic once a response has
    /// been received
    /// </summary>
    [TestClass]
    public class AirportLookupTest
    {
        private const string EndpointBaseUrl = "https://aerodatabox.p.rapidapi.com/airports/iata/";
        private const string AirportCode = "LHR";
        private const string MalformedResponse = "[{\"fullName\": \"London Heathrow\",\"country\": {\"name\": \"United Kingdom\"}}]";
        private const string Response = "{\"fullName\": \"London Heathrow\",\"country\": {\"name\": \"United Kingdom\"}}";

        private MockHttpClient _client;
        private IAirportsApi _api;

        [TestInitialize]
        public void Initialise()
        {
            // Create the logger, mock client and API wrappers
            var logger = new MockFileLogger();
            _client = new MockHttpClient();
            _api = new AeroDataBoxAirportsApi(logger, _client, EndpointBaseUrl, "");
        }

        [TestMethod]
        public void AirportNotFoundTest()
        {
            _client!.AddResponse("");
            var properties = Task.Run(() => _api.LookupAirportByIATACode("Airport Doesn't Exist")).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void NetworkErrorTest()
        {
            _client!.AddResponse(null);
            var properties = Task.Run(() => _api.LookupAirportByIATACode(AirportCode)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void MalformedResponseTest()
        {
            _client!.AddResponse(MalformedResponse);
            var properties = Task.Run(() => _api.LookupAirportByIATACode(AirportCode)).Result;
            Assert.IsNull(properties);
        }

        [TestMethod]
        public void LookupAirportByIATACodeTest()
        {
            _client!.AddResponse(Response);
            var properties = Task.Run(() => _api.LookupAirportByIATACode(AirportCode)).Result;
            Assert.IsNotNull(properties);

            var airport = properties[ApiPropertyType.AirportName];
            var country = properties[ApiPropertyType.CountryName];

            Assert.AreEqual("London Heathrow", airport);
            Assert.AreEqual("United Kingdom", country);
        }
    }
}
