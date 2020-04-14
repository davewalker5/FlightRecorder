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
    public class ModelsController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public ModelsController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("manufacturer/{manufacturerId}")]
        public async Task<ActionResult<List<Model>>> GetModelsAsync(int manufacturerId)
        {
            List<Model> models = await _factory.Models
                                               .ListAsync(m => m.ManufacturerId == manufacturerId)
                                               .ToListAsync();

            if (!models.Any())
            {
                return NoContent();
            }

            return models;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Model>> GetModelAsync(int id)
        {
            Model model = await _factory.Models.GetAsync(m => m.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            // TODO : This logic should be in the business logic
            await _factory.Context.Entry(model).Reference(m => m.Manufacturer).LoadAsync();

            return model;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Model>> UpdateModelAsync([FromBody] Model template)
        {
            // TODO : Move this functionality to the Business Logic assembly
            Model model = await _factory.Models
                                        .GetAsync(m => (m.Name == template.Name) &&
                                                       (m.ManufacturerId == template.ManufacturerId));
            if (model != null)
            {
                return BadRequest();
            }

            model = await _factory.Models.GetAsync(m => m.Id == template.Id);
            if (model == null)
            {
                return NotFound();
            }

            model.Name = template.Name;
            model.ManufacturerId = template.ManufacturerId;
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(model).Reference(m => m.Manufacturer).LoadAsync();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Model>> CreateModelAsync([FromBody] Model template)
        {
            // TODO : Should have an add method using the manufacturer ID
            Model location = await _factory.Models.AddAsync(template.Name, template.Manufacturer.Name);
            return location;
        }
    }
}
