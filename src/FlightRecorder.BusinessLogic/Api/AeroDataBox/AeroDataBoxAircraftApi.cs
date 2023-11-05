using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Api.AeroDataBox
{
    public partial class AeroDataBoxAircraftApi : ExternalApiBase, IAircraftApi
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const double DaysPerYear = 365.2425;

        private readonly string _baseAddress;
        private readonly string _host;
        private readonly string _key;

        public AeroDataBoxAircraftApi(
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
        /// Look up aircraft details given a registration number
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public async Task<Dictionary<ApiPropertyType, string>> LookupAircraftByRegistration(string registration)
        {
            Logger.LogMessage(Severity.Info, $"Looking up aircraft with registration number {registration}");
            var properties = await MakeApiRequest($"reg/{registration}");
            return properties;
        }

        /// <summary>
        /// Look up aircraft details given a 24-bit ICAO address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<Dictionary<ApiPropertyType, string>> LookupAircraftByICAOAddress(string address)
        {
            Logger.LogMessage(Severity.Info, $"Looking up aircraft with 24-bit ICAO address {address}");
            var properties = await MakeApiRequest($"icao24/{address}");
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
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftRegistration, JsonPath = "reg" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftICAOAddress, JsonPath = "hexIcao" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftSerialNumber, JsonPath = "serial" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftRegistrationDate, JsonPath = "registrationDate" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftModel, JsonPath = "model" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftModelCode, JsonPath = "modelCode" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftType, JsonPath = "typeName" },
                new ApiPropertyDefinition{ PropertyType = ApiPropertyType.AircraftProductionLine, JsonPath = "productionLine" },
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

                    // Calculate the age of the aircraft and add it to the properties
                    var age = CalculateAircraftAge(properties[ApiPropertyType.AircraftRegistrationDate]);
                    properties.Add(ApiPropertyType.AircraftAge, age?.ToString() ?? "");

                    // Determine the manufacturer from the type name and model code
                    var manufacturer = DetermineManufacturer(properties);
                    properties.Add(ApiPropertyType.ManufacturerName, manufacturer);

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

        /// <summary>
        /// Calculate the age of an aircraft from its registration date
        /// </summary>
        /// <param name="yearOfRegistration"></param>
        /// <returns></returns>
        private static int? CalculateAircraftAge(string yearOfRegistration)
        {
            int? age = null;

            try
            {
                // Convert the registration date string to a date then calculate the number of years between then and now
                var registered = DateTime.ParseExact(yearOfRegistration, DateFormat, null);
                age = (int)Math.Round((DateTime.Now - registered).TotalDays / DaysPerYear, 0, MidpointRounding.AwayFromZero);
            }
            catch
            {
                // Malformed year of registration, so we can't return an age
            }

            return age;
        }

        /// <summary>
        /// Determine the aircraft manufacturer's name given the model type name and the production line name
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static string DetermineManufacturer(Dictionary<ApiPropertyType, string> properties)
        {
            var builder = new StringBuilder();
            var numbers = MyRegex();

            // Get the properties of interest from the properties returned by the API
            var modelTypeName = properties[ApiPropertyType.AircraftType];
            var productionLine = properties[ApiPropertyType.AircraftProductionLine];

            // Check the strings have some content
            if (!string.IsNullOrEmpty(modelTypeName) && !string.IsNullOrEmpty(productionLine))
            {
                // The manufacturer can be inferred from the properties returned from the API:
                //
                // Type Name: Boeing 737-800
                // Production Line: Boeing 737 NG
                //
                // It's the (trimmed) part of the two strings that are identical and (from other
                // examples) don't contain numbers, which are unlikely (though not impossible) in
                // a manufacturer name

                // Split the two into words
                var typeWords = modelTypeName.Split(' ');
                var lineWords = productionLine.Split(' ');

                // Use a string builder to build up a string containing only the parts where the words match
                for (int i = 0; i < typeWords.Length; i++)
                {
                    // Compare the word at the current position in the type and production line strings
                    if (typeWords[i].Equals(lineWords[i], StringComparison.OrdinalIgnoreCase) && !numbers.Match(typeWords[i]).Success)
                    {
                        // The same and not containing numbers, so add this word to the builder (with a preceding
                        // space if it's not the first word)
                        if (builder.Length > 0) builder.Append(' ');
                        builder.Append(typeWords[i]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Regular expression to find numbers
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("[0-9]", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }
}