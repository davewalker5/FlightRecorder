using System;
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
    public class SightingsController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";
        private readonly FlightRecorderFactory _factory;

        public SightingsController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("route/{embarkation}/{destination}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByRouteAsync(string embarkation, string destination)
        {
            string decodedEmbarkation = HttpUtility.UrlDecode(embarkation).ToUpper();
            string decodedDestination = HttpUtility.UrlDecode(destination).ToUpper();
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => (s.Flight.Embarkation == decodedEmbarkation) &&
                                                                     (s.Flight.Destination == decodedDestination))
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("flight/{number}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByFlightAsync(string number)
        {
            string decodedNumber = HttpUtility.UrlDecode(number).ToUpper();
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => s.Flight.Number == decodedNumber)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("airline/{airlineId}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByAirlineAsync(int airlineId)
        {
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => s.Flight.AirlineId == airlineId)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("aircraft/{aircraftId}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByAircraftAsync(int aircraftId)
        {
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => s.AircraftId == aircraftId)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("date/{start}/{end}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByDateAsync(string start, string end)
        {
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => (s.Date >= startDate) &&
                                                                     (s.Date <= endDate))
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Sighting>> GetSightingAsync(int id)
        {
            Sighting sighting = await _factory.Sightings.GetAsync(m => m.Id == id);

            if (sighting == null)
            {
                return NotFound();
            }

            // TODO : This logic should be in the business logic
            await _factory.Context.Entry(sighting).Reference(s => s.Aircraft).LoadAsync();
            await _factory.Context.Entry(sighting.Aircraft).Reference(m => m.Model).LoadAsync();
            await _factory.Context.Entry(sighting.Aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();
            await _factory.Context.Entry(sighting).Reference(s => s.Flight).LoadAsync();
            await _factory.Context.Entry(sighting.Flight).Reference(f => f.Airline).LoadAsync();
            await _factory.Context.Entry(sighting).Reference(s => s.Location).LoadAsync();

            return sighting;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Sighting>> UpdateSightingAsync([FromBody] Sighting template)
        {
            // TODO This logic should be in the business logic
            Sighting sighting = await _factory.Sightings.GetAsync(s => s.Id == template.Id);
            if (sighting == null)
            {
                return NotFound();
            }

            sighting.Date = template.Date;
            sighting.AircraftId = template.AircraftId;
            sighting.Altitude = template.Altitude;
            sighting.FlightId = template.FlightId;
            sighting.LocationId = template.LocationId;
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(sighting).Reference(s => s.Aircraft).LoadAsync();
            await _factory.Context.Entry(sighting.Aircraft).Reference(m => m.Model).LoadAsync();
            await _factory.Context.Entry(sighting.Aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();
            await _factory.Context.Entry(sighting).Reference(s => s.Flight).LoadAsync();
            await _factory.Context.Entry(sighting.Flight).Reference(f => f.Airline).LoadAsync();
            await _factory.Context.Entry(sighting).Reference(s => s.Location).LoadAsync();

            return sighting;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Sighting>> CreateSightingAsync([FromBody] Sighting template)
        {
            Sighting location = await _factory.Sightings
                                              .AddAsync(template.Altitude,
                                                        template.Date,
                                                        template.LocationId,
                                                        template.FlightId,
                                                        template.AircraftId);
            return location;
        }
    }
}
