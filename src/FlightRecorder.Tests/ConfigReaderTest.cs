using FlightRecorder.BusinessLogic.Config;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FlightRecorder.Tests
{
    [TestClass]
    public class ConfigReaderTest
    {
        [TestMethod]
        public void ReadFlightRecorderApplicationSettingsTest()
        {
            var settings = new FlightRecorderConfigReader().Read("appsettings.json", "ApplicationSettings");

            Assert.AreEqual("my-secret", settings.Secret);
            Assert.AreEqual(60, settings.TokenLifespanMinutes);
            Assert.AreEqual("Export", settings.SightingsExportPath);
            Assert.AreEqual(@"Export\Airports", settings.AirportsExportPath);
            Assert.AreEqual(@"Export\Reports", settings.ReportsExportPath);
            Assert.AreEqual("FlightRecorder.log", settings.LogFile);
            Assert.AreEqual(Severity.Info, settings.MinimumLogLevel);

            Assert.IsNotNull(settings.ApiEndpoints);
            Assert.AreEqual(1, settings.ApiEndpoints.Count);
            Assert.AreEqual(ApiEndpointType.Flight, settings.ApiEndpoints.First().EndpointType);
            Assert.AreEqual(ApiServiceType.AeroDataBox, settings.ApiEndpoints.First().Service);
            Assert.AreEqual("https://aerodatabox.p.rapidapi.com/flights/number/", settings.ApiEndpoints.First().Url);

            Assert.IsNotNull(settings.ApiServiceKeys);
            Assert.AreEqual(1, settings.ApiServiceKeys.Count);
            Assert.AreEqual(ApiServiceType.AeroDataBox, settings.ApiServiceKeys.First().Service);
            Assert.AreEqual("my-key", settings.ApiServiceKeys.First().Key);
        }

        [TestMethod]
        public void ResolveKeyFromFileTest()
        {
            var settings = new FlightRecorderConfigReader().Read("separatekeyfile.json", "ApplicationSettings");

            Assert.IsNotNull(settings.ApiServiceKeys);
            Assert.AreEqual(1, settings.ApiServiceKeys.Count);
            Assert.AreEqual(ApiServiceType.AeroDataBox, settings.ApiServiceKeys.First().Service);
            Assert.AreEqual("key-from-file", settings.ApiServiceKeys.First().Key);
        }
    }
}
