using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Api.AeroDataBox
{
    public class AeroDataBoxAirportsApi : ExternalApiBase, IAirportsApi
    {
        private readonly string _baseAddress;
        private readonly string _host;
        private readonly string _key;

        public AeroDataBoxAirportsApi(
            IFlightRecorderLogger logger,
            IFlightRecorderHttpClient client,
            string url,
            string key)
            : base(logger, client)
        {
            _baseAddress = url;
            _key = key;

            // The URL contains the protocol, host and base route (if any), but we need to extract the host name only
            // to pass in the headers as the RapidAPI host, so capture the host and the full URL
            Uri uri = new(url);
            _host = uri.Host;
        }

        /// <summary>
        /// Look up airport details given an airport IATA code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Dictionary<ApiPropertyType, string>> LookupAirportByIATACode(string code)
        {
            Logger.LogMessage(Severity.Info, $"Looking up airport {code}");
            var properties = await MakeApiRequest(code);
            return properties;
        }

        /// <summary>
        /// Make a request for flight details using the specified parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task<Dictionary<ApiPropertyType, string>> MakeApiRequest(string parameters)
        {
            Dictionary<ApiPropertyType, string> properties = null;

            // Define the properties to be extracted from the response
            List<ApiPropertyDefinition> definitions = new()
            {
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AirportName, JsonPath = "fullName" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.CountryName, JsonPath = "country.name" }
            };

            // Set the headers
            SetHeaders(new Dictionary<string, string>
            {
                { "X-RapidAPI-Host", _host },
                { "X-RapidAPI-Key", _key }
            });

            // Make a request for the data from the API
            var url = $"{_baseAddress}{parameters}";
            var node = await SendRequest(url);

            if (node != null)
            {
                try
                {
                    // Extract the required properties from the response
                    properties = GetPropertyValuesFromResponse(node, definitions);

                    // Log the properties dictionary
                    LogProperties(properties!);
                }
                catch (Exception ex)
                {
                    var message = $"Error processing response: {ex.Message}";
                    Logger.LogMessage(Severity.Error, message);
                    Logger.LogException(ex);
                    properties = null;
                }
            }

            return properties;
        }
    }
}