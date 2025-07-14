using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class LocationsController : FlightRecorderApiController
    {
        public LocationsController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Location>>> GetLocationsAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of locations (page {pageNumber}, page size {pageSize})");

            List<Location> locations = await Factory.Locations
                                                     .ListAsync(null, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {locations.Count} location(s)");

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
            LogMessage(Severity.Debug, $"Retrieving location with ID {id}");

            Location location = await Factory.Locations
                                              .GetAsync(m => m.Id == id);

            if (location == null)
            {
                LogMessage(Severity.Debug, $"Location with ID {id} not found");
                return NotFound();
            }

            return location;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Location>> AddLocationAsync([FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Adding location: Name = {name}");
            Location location = await Factory.Locations.AddAsync(name);
            return location;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Location>> UpdateLocationAsync(int id, [FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Updating location: ID = {id}, Name = {name}");
            var location = await Factory.Locations.UpdateAsync(id, name);
            return location;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteLocationAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting location: ID = {id}");
            await Factory.Locations.DeleteAsync(id);
            return Ok();
        }
    }
}
