using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Api.AeroDataBox
{
    public class AeroDataBoxFlightsApi : ExternalApiBase, IFlightsApi
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly string _baseAddress;
        private readonly string _host;
        private readonly string _key;

        public AeroDataBoxFlightsApi(
            IFlightRecorderLogger logger,
            IExternalApiHttpClient client,
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
        /// Look up flight details given a flight number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Dictionary<ApiPropertyType, string>> LookupFlightByNumber(string number)
        {
            Logger.LogMessage(Severity.Info, $"Looking up flight {number}");
            var properties = await MakeApiRequest(number);
            return properties;
        }

        /// <summary>
        /// Look up flight details given a flight number and date
        /// </summary>
        /// <param name="number"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<Dictionary<ApiPropertyType, string>> LookupFlightByNumberAndDate(string number, DateTime date)
        {
            Logger.LogMessage(Severity.Info, $"Looking up flight {number} on {date}");
            var parameters = $"{number}/{date.ToString(DateFormat)}";
            var properties = await MakeApiRequest(parameters);
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
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.DepartureAirportIATA, JsonPath = "departure.airport.iata" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.DestinationAirportIATA, JsonPath = "arrival.airport.iata" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AirlineName, JsonPath = "airline.name" }
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
                    properties = GetPropertyValuesFromResponse(node![0], definitions);

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
