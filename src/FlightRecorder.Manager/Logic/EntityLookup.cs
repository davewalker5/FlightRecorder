using FlightRecorder.BusinessLogic.Api;
using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Manager.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FlightRecorder.Manager.Logic
{
    public class EntityLookup
    {
        private readonly IFlightRecorderLogger _logger;
        private readonly IEnumerable<ApiEndpoint> _endpoints;
        private readonly IEnumerable<ApiServiceKey> _keys;
        private readonly IFlightRecorderHttpClient _client = FlightRecorderHttpClient.Instance;

        public EntityLookup(IFlightRecorderLogger logger, IEnumerable<ApiEndpoint> endpoints, IEnumerable<ApiServiceKey> keys)
        {
            _logger = logger;
            _endpoints = endpoints;
            _keys = keys;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task LookupEntity(CommandLineEntityType type, string identifier)
        {
            switch (type)
            {
                case CommandLineEntityType.flight:
                    await LookupFlight(identifier);
                    break;
                case CommandLineEntityType.airport:
                    await LookupAirport(identifier);
                    break;
                case CommandLineEntityType.aircraft:
                    await LookupAircraft(identifier);
                    break;
                default:
                    Console.WriteLine($"Invalid entity type for lookup: {type.ToString()}");
                    break;
            }
        }

        /// <summary>
        /// Get the description/display name for an API property type
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static string GetPropertyDescription(ApiPropertyType propertyType)
        {
            // Get the field information for the property type
            var type = typeof(ApiPropertyType);
            var info = type.GetField(propertyType.ToString());

            // Get the description attribute for the property type
            var attribute = Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute)) as DescriptionAttribute;

            // If we have an attribute, use its description. Otherwise, default to the property type name
            var description = attribute?.Description ?? propertyType.ToString();
            return description;
        }

        /// <summary>
        /// Write a set of properties from the API to the console
        /// </summary>
        /// <param name="properties"></param>
        private static void WritePropertiesToConsole(string entity, string identifier, Dictionary<ApiPropertyType, string> properties)
        {
            // Write the identifier used to lookup the data
            Console.WriteLine($"\n{entity} {identifier}:\n");

            // Check we have some properties
            if ((properties != null) && (properties.Count > 0))
            {
                // Iterate over the properties and write the key and value to the console
                foreach (var property in properties)
                {
                    var description = GetPropertyDescription(property.Key);
                    Console.WriteLine($"{description} = {property.Value}");
                }
            }
            else
            {
                Console.WriteLine("No properties returned by the API");
            }
        }

        /// <summary>
        /// Lookup flight details and write them to the console
        /// </summary>
        /// <param name="iataCode"></param>
        /// <returns></returns>
        private async Task LookupFlight(string number)
        {
            // Get the API key and endpoint URL
            var key = _keys.First(x => x.Service == ApiServiceType.AeroDataBox).Key;
            var url = _endpoints.First(x => x.EndpointType == ApiEndpointType.Flight).Url;

            // Create an instance of the API and call it to look up the flight
            var api = new AeroDataBoxFlightsApi(_logger, _client, url, key);
            var properties = await api.LookupFlightByNumber(number);

            // Write the properties to the console
            WritePropertiesToConsole("Flight", number, properties);
        }

        /// <summary>
        /// Lookup airport details and write them to the console
        /// </summary>
        /// <param name="iata"></param>
        /// <returns></returns>
        private async Task LookupAirport(string iata)
        {
            // Get the API key and endpoint URL
            var key = _keys.First(x => x.Service == ApiServiceType.AeroDataBox).Key;
            var url = _endpoints.First(x => x.EndpointType == ApiEndpointType.Airport).Url;

            // Create an instance of the API and call it to look up the flight
            var api = new AeroDataBoxAirportsApi(_logger, _client, url, key);
            var properties = await api.LookupAirportByIATACode(iata);

            // Write the properties to the console
            WritePropertiesToConsole("Airport", iata, properties);
        }

        /// <summary>
        /// Lookup aircraft details and write them to the console
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        private async Task LookupAircraft(string registration)
        {
            // Get the API key and endpoint URL
            var key = _keys.First(x => x.Service == ApiServiceType.AeroDataBox).Key;
            var url = _endpoints.First(x => x.EndpointType == ApiEndpointType.Aircraft).Url;

            // Create an instance of the API and call it to look up the flight
            var api = new AeroDataBoxAircraftApi(_logger, _client, url, key);
            var properties = await api.LookupAircraftByRegistration(registration);

            // Write the properties to the console
            WritePropertiesToConsole("Registration", registration, properties);
        }
    }
}
