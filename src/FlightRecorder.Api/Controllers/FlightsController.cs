using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class FlightsController : FlightRecorderApiController
    {
        public FlightsController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("route/{embarkation}/{destination}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsByRouteAsync(string embarkation, string destination, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of flights by route {embarkation}-{destination} (page {pageNumber}, page size {pageSize})");

            string decodedEmbarkation = HttpUtility.UrlDecode(embarkation).ToUpper();
            string decodedDestination = HttpUtility.UrlDecode(destination).ToUpper();
            List<Flight> flights = await Factory.Flights
                                                 .ListAsync(f => (f.Embarkation == decodedEmbarkation) &&
                                                                 (f.Destination == decodedDestination), pageNumber, pageSize)
                                                 .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {flights.Count} flights(s)");

            if (!flights.Any())
            {
                return NoContent();
            }

            // Assign airport details
            await Factory.Airports.LoadAirportDetails(flights);

            return flights;
        }

        [HttpGet]
        [Route("airline/{airlineId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsByAirlineAsync(int airlineId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of flights with airline ID {airlineId} (page {pageNumber}, page size {pageSize})");

            List<Flight> flights = await Factory.Flights
                                                 .ListAsync(f => f.AirlineId == airlineId, pageNumber, pageSize)
                                                 .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {flights.Count} flights(s)");

            if (!flights.Any())
            {
                return NoContent();
            }

            // Assign airport details
            await Factory.Airports.LoadAirportDetails(flights);

            return flights;
        }

        [HttpGet]
        [Route("number/{number}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsByNumberAsync(string number, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of flights with number {number} (page {pageNumber}, page size {pageSize})");

            string decodedNumber = HttpUtility.UrlDecode(number).ToUpper();
            List<Flight> flights = await Factory.Flights
                                                 .ListAsync(f => f.Number == decodedNumber, pageNumber, pageSize)
                                                 .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {flights.Count} flights(s)");

            if (!flights.Any())
            {
                return NoContent();
            }

            // Assign airport details
            await Factory.Airports.LoadAirportDetails(flights);

            return flights;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Flight>> GetFlightByIdAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving flight with ID {id}");

            Flight flight = await Factory.Flights.GetAsync(f => f.Id == id);

            if (flight == null)
            {
                LogMessage(Severity.Debug, $"Flight with ID {id} not found");
                return NotFound();
            }

            // Assign airport details
            await Factory.Airports.LoadAirportDetails([flight]);

            return flight;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Flight>> AddFlightAsync([FromBody] Flight template)
        {
            LogMessage(Severity.Debug, $"Adding flight: {template}");
            Flight flight = await Factory.Flights
                                          .AddAsync(template.Number,
                                                    template.Embarkation,
                                                    template.Destination,
                                                    template.AirlineId);
            LogMessage(Severity.Debug, $"Added flight: {flight}");

            // Assign airport details
            await Factory.Airports.LoadAirportDetails([flight]);
            return flight;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Flight>> UpdateFlightAsync([FromBody] Flight template)
        {
            LogMessage(Severity.Debug, $"Updating flight: {template}");

            Flight flight = await Factory.Flights.UpdateAsync(
                template.Id,
                template.Number,
                template.Embarkation,
                template.Destination,
                template.AirlineId);

            // Assign airport details
            await Factory.Airports.LoadAirportDetails([flight]);

            LogMessage(Severity.Debug, $"Flight updated: {flight}");

            return flight;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFlightAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting flight: ID = {id}");
            await Factory.Flights.DeleteAsync(id);
            return Ok();
        }
    }
}
