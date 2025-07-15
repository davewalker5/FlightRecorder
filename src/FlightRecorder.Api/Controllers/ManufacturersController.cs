using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
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
    public class ManufacturersController : FlightRecorderApiController
    {
        public ManufacturersController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Manufacturer>>> GetManufacturersAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of manufacturers (page {pageNumber}, page size {pageSize})");

            List<Manufacturer> manufacturers = await Factory.Manufacturers
                                                             .ListAsync(null, pageNumber, pageSize)
                                                             .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {manufacturers.Count} manufacturer(s)");

            if (!manufacturers.Any())
            {
                return NoContent();
            }

            return manufacturers;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Manufacturer>> GetManufacturerAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving manufacturer with ID {id}");

            Manufacturer manufacturer = await Factory.Manufacturers
                                                      .GetAsync(m => m.Id == id);

            if (manufacturer == null)
            {
                LogMessage(Severity.Debug, $"Manufacturer with ID {id} not found");
                return NotFound();
            }

            return manufacturer;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Manufacturer>> AddManufacturerAsync([FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Adding manufacturer: Name = {name}");
            Manufacturer manufacturer = await Factory.Manufacturers.AddAsync(name);
            LogMessage(Severity.Debug, $"Added manufacturer: {manufacturer}");
            return manufacturer;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Manufacturer>> UpdateManufacturerAsync(int id, [FromBody] string name)
        {

            LogMessage(Severity.Debug, $"Updating manufacturer: ID = {id}, Name = {name}");
            Manufacturer manufacturer = await Factory.Manufacturers.UpdateAsync(id, name);
            LogMessage(Severity.Debug, $"Manufacturer updated: {manufacturer}");
            return manufacturer;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteManufacturerAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting manufacturer: ID = {id}");
            await Factory.Manufacturers.DeleteAsync(id);
            return Ok();
        }
    }
}
