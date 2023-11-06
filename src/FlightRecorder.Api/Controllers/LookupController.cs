using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.Entities.Api;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Web;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class LookupController : Controller
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly IFlightsApi _flights;
        private readonly IAirportsApi _airports;
        private readonly IAircraftApi _aircraft;

        public LookupController(IFlightsApi flights, IAirportsApi airports, IAircraftApi aircraft)
        {
            _flights = flights;
            _airports = airports;
            _aircraft = aircraft;
        }

        /// <summary>
        /// Lookup a flight given the flight number and date
        /// </summary>
        /// <param name="number"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("flight/{number}/{date}")]
        public async Task<ActionResult<Flight>> LookupFlight(string number, string date)
        {
            // Decode the date and convert them to dates
            var dateOfFlight = DateTime.ParseExact(HttpUtility.UrlDecode(date), DateFormat, null);

            // Lookup the flight
            var properties = await _flights.LookupFlightByNumberAndDate(number, dateOfFlight);

            if (properties == null)
            {
                return NoContent();
            }

            // Create the flight and associated airline from the properties and return the flight
            var flight = new Flight
            {
                Embarkation = properties[ApiPropertyType.DepartureAirportIATA],
                Destination = properties[ApiPropertyType.DestinationAirportIATA],
                Airline = new Airline
                {
                    Name = properties[ApiPropertyType.AirlineName]
                }
            };

            return flight;
        }

        /// <summary>
        /// Lookup an airport given its IATA code
        /// </summary>
        /// <param name="iata"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("airport/{iata}")]
        public async Task<ActionResult<Airport>> LookupAirport(string iata)
        {
            // Lookup the airport
            var properties = await _airports.LookupAirportByIATACode(iata);

            if (properties == null)
            {
                return NoContent();
            }

            // Create the airport and associated country from the properties and return the airport
            var airport = new Airport
            {
                Code = iata,
                Name = properties[ApiPropertyType.AirportName],
                Country = new Country
                {
                    Name = properties[ApiPropertyType.CountryName]
                }
            };

            return airport;
        }

        /// <summary>
        /// Lookup an aircraft given its registration number
        /// </summary>
        /// <param name="iata"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("aircraft/{registration}")]
        public async Task<ActionResult<Aircraft>> LookupAircraft(string registration)
        {
            // Lookup the airport
            var properties = await _aircraft.LookupAircraftByRegistration(registration);

            if (properties == null)
            {
                return NoContent();
            }

            // Create the aircraft and associated model and manufacturer from the properties and return the aircraft
            int.TryParse(properties[ApiPropertyType.AircraftAge], out int age);
            var aircraft = new Aircraft
            {
                Registration = registration,
                // Currently, the age calculation isn't working (variable results, based on the aircraft being looked up)
                // Manufactured = DateTime.Now.AddYears(-age).Year,
                SerialNumber = properties[ApiPropertyType.AircraftSerialNumber],
                Model = new Model
                {
                    Name = properties[ApiPropertyType.AircraftModelCode],
                    Manufacturer = new Manufacturer
                    {
                        Name = properties[ApiPropertyType.ManufacturerName]
                    }
                }
            };

            return aircraft;
        }
    }
}
