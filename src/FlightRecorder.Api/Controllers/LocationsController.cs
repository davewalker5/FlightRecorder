using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LocationsController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public LocationsController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Location>>> GetLocationsAsync(int pageNumber, int pageSize)
        {
            List<Location> locations = await _factory.Locations
                                                     .ListAsync(null, pageNumber, pageSize).ToListAsync();

            if (!locations.Any())
            {
                return NoContent();
            }

            return locations;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Location>> GetLocationAsync(int id)
        {
            Location location = await _factory.Locations
                                              .GetAsync(m => m.Id == id);

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Location>> UpdateLocationAsync(int id, [FromBody] string name)
        {
            // TODO : Move this functionality to the Business Logic assembly
            Location location = await _factory.Locations
                                              .GetAsync(m => m.Name == name);
            if (location != null)
            {
                return BadRequest();
            }

            location = await _factory.Locations
                                     .GetAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            location.Name = name;
            await _factory.Context.SaveChangesAsync();

            return location;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Location>> CreateLocationAsync([FromBody] string name)
        {
            Location location = await _factory.Locations.AddAsync(name);
            return location;
        }
    }
}
