using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Api
{
    public abstract class ExternalApiBase
    {
        private readonly IExternalApiHttpClient _client;
        protected IFlightRecorderLogger Logger { get; private set; }

        protected ExternalApiBase(IFlightRecorderLogger logger, IExternalApiHttpClient client)
        {
            Logger = logger;
            _client = client;
        }

        /// <summary>
        /// Set the request headers
        /// </summary>
        /// <param name="headers"></param>
        protected virtual void SetHeaders(Dictionary<string, string> headers)
            => _client.SetHeaders(headers);

        /// <summary>
        /// Make a request to the specified URL and return the response properties as a JSON DOM
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        protected virtual async Task<JsonNode> SendRequest(string endpoint)
        {
            JsonNode node = null;

            try
            {
                // Make a request for the data from the API
                using (var response = await _client.GetAsync(endpoint))
                {
                    // Check the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response, parse to a JSON DOM
                        var json = await response.Content.ReadAsStringAsync();
                        node = JsonNode.Parse(json);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Error calling {endpoint}: {ex.Message}";
                Logger.LogMessage(Severity.Error, message);
                Logger.LogException(ex);
                node = null;
            }

            return node;
        }

        /// <summary>
        /// Given a JSON node and the path to an element, return the value at that element
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetPropertyValueByPath(JsonNode node, ApiPropertyDefinition definition)
        {
            string value = null;
            var current = node;

            // Walk the JSON document to the requested element
            foreach (var element in definition.JsonPath.Split(".", StringSplitOptions.RemoveEmptyEntries))
            {
                current = current?[element];
            }

            // Check the element is a type that can yield a value
            if (current is JsonValue)
            {
                // Extract the value as a string and if "cleanup" has been specified perform it
                value = current?.GetValue<string>();
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="propertyDefinitions"></param>
        protected virtual Dictionary<ApiPropertyType, string> GetPropertyValuesFromResponse(JsonNode node, IEnumerable<ApiPropertyDefinition> propertyDefinitions)
        {
            var properties = new Dictionary<ApiPropertyType, string>();

            // Iterate over the property definitions
            foreach (var definition in propertyDefinitions)
            {
                // Get the value from
                var value = GetPropertyValueByPath(node, definition);
                properties.Add(definition.PropertyType, value ?? "");
            }

            // Log the properties dictionary
            LogProperties(properties!);

            return properties;
        }

        /// <summary>
        /// Log the content of a properties dictionary resulting from an external API call
        /// </summary>
        /// <param name="properties"></param>
        [ExcludeFromCodeCoverage]
        protected void LogProperties(Dictionary<ApiPropertyType, string> properties)
        {
            // Check the properties dictionary isn't NULL
            if (properties != null)
            {
                // Not a NULL dictionary, so iterate over all the properties it contains
                foreach (var property in properties)
                {
                    // Construct a message containing the property name and the value, replacing
                    // null values with "NULL"
                    var value = property.Value != null ? property.Value.ToString() : "NULL";
                    var message = $"API property {property.Key.ToString()} = {value}";

                    // Log the message for this property
                    Logger.LogMessage(Severity.Info, message);
                }
            }
            else
            {
                // Log the fact that the properties dictionary is NULL
                Logger.LogMessage(Severity.Warning, "API lookup generated a NULL properties dictionary");
            }
        }
    }
}
