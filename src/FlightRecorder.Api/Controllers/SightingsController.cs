using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

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
        [Route("route/{embarkation}/{destination}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByRouteAsync(string embarkation, string destination, int pageNumber, int pageSize)
        {
            string decodedEmbarkation = HttpUtility.UrlDecode(embarkation).ToUpper();
            string decodedDestination = HttpUtility.UrlDecode(destination).ToUpper();
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => (s.Flight.Embarkation == decodedEmbarkation) &&
                                                                     (s.Flight.Destination == decodedDestination), pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("flight/{number}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByFlightAsync(string number, int pageNumber, int pageSize)
        {
            string decodedNumber = HttpUtility.UrlDecode(number).ToUpper();
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => s.Flight.Number == decodedNumber, pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("airline/{airlineId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByAirlineAsync(int airlineId, int pageNumber, int pageSize)
        {
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => s.Flight.AirlineId == airlineId, pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("aircraft/{aircraftId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByAircraftAsync(int aircraftId, int pageNumber, int pageSize)
        {
            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => s.AircraftId == aircraftId, pageNumber, pageSize)
                                                     .ToListAsync();

            if (!sightings.Any())
            {
                return NoContent();
            }

            return sightings;
        }

        [HttpGet]
        [Route("date/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Sighting>>> GetSightingsByDateAsync(string start, string end, int pageNumber, int pageSize)
        {
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            List<Sighting> sightings = await _factory.Sightings
                                                     .ListAsync(s => (s.Date >= startDate) &&
                                                                     (s.Date <= endDate), pageNumber, pageSize)
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
 
        [HttpGet]
        [Route("recent/flight/{number}")]
        public async Task<ActionResult<Sighting>> GetMostRecentFlightSightingAsync(string number)
        {
            string decodedNumber = HttpUtility.UrlDecode(number).ToUpper();
            Sighting sighting = await _factory.Sightings.GetMostRecent(x => x.Flight.Number == decodedNumber);

            if (sighting == null)
            {
                return NoContent();
            }

            return sighting;
        }

        [HttpGet]
        [Route("recent/aircraft/{registration}")]
        public async Task<ActionResult<Sighting>> GetMostRecentAircraftSightingAsync(string registration)
        {
            string decodedRegistration = HttpUtility.UrlDecode(registration);
            Sighting sighting = await _factory.Sightings.GetMostRecent(x => x.Aircraft.Registration == decodedRegistration);

            if (sighting == null)
            {
                return NoContent();
            }

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
            sighting.IsMyFlight = template.IsMyFlight;
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
                                                        template.AircraftId,
                                                        template.IsMyFlight);
            return location;
        }
    }
}
