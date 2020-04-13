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
    public class ManufacturersController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public ManufacturersController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Manufacturer>>> GetManufacturersAsync()
        {
            List<Manufacturer> manufacturers = await _factory.Manufacturers.ListAsync().ToListAsync();

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
            Manufacturer manufacturer = await _factory.Manufacturers.GetAsync(m => m.Id == id);

            if (manufacturer == null)
            {
                return NotFound();
            }

            return manufacturer;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Manufacturer>> UpdateManufacturerAsync(int id, [FromBody] string name)
        {
            // TODO : Move this functionality to the Business Logic assembly
            Manufacturer manufacturer = await _factory.Manufacturers.GetAsync(m => m.Name == name);
            if (manufacturer != null)
            {
                return BadRequest();
            }

            manufacturer = await _factory.Manufacturers.GetAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            manufacturer.Name = name;
            await _factory.Context.SaveChangesAsync();

            return manufacturer;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Manufacturer>> CreateManufacturerAsync([FromBody] string name)
        {
            Manufacturer manufacturer = await _factory.Manufacturers.AddAsync(name);
            return manufacturer;
        }
    }
}
