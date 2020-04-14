using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class FlightsController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public FlightsController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("route/{embarkation}/{destination}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsByRouteAsync(string embarkation, string destination)
        {
            string decodedEmbarkation = HttpUtility.UrlDecode(embarkation).ToUpper();
            string decodedDestination = HttpUtility.UrlDecode(destination).ToUpper();
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(f => (f.Embarkation == decodedEmbarkation) &&
                                                                 (f.Destination == decodedDestination))
                                                 .ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("airline/{airlineId}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsByAirlineAsync(int airlineId)
        {
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(f => f.AirlineId == airlineId)
                                                 .ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("number/{number}")]
        public async Task<ActionResult<List<Flight>>> GetFlightsByNumberAsync(string number)
        {
            string decodedNumber = HttpUtility.UrlDecode(number).ToUpper();
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(f => f.Number == decodedNumber)
                                                 .ToListAsync();

            if (!flights.Any())
            {
                return NoContent();
            }

            return flights;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Flight>> GetFlightByIdAsync(int id)
        {
            Flight flight = await _factory.Flights.GetAsync(f => f.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            // TODO : This logic should be in the business logic
            await _factory.Context.Entry(flight).Reference(f => f.Airline).LoadAsync();

            return flight;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Flight>> UpdateFlightAsync([FromBody] Flight template)
        {
            Flight flight = await _factory.Flights.GetAsync(f => f.Id == template.Id);
            if (flight == null)
            {
                return NotFound();
            }

            flight.Number = template.Number.ToUpper();
            flight.Embarkation = template.Embarkation.ToUpper();
            flight.Destination = template.Destination.ToUpper();
            flight.AirlineId = template.AirlineId;
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(flight).Reference(a => a.Airline).LoadAsync();

            return flight;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Flight>> CreateFlightAsync([FromBody] Flight template)
        {
            // TODO : Should have a method taking the airline ID
            Flight flight = await _factory.Flights
                                          .AddAsync(template.Number,
                                                    template.Embarkation,
                                                    template.Destination,
                                                    template.Airline.Name);
            return flight;
        }
    }
}
