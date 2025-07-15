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
    public class ModelsController : FlightRecorderApiController
    {
        public ModelsController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("manufacturer/{manufacturerId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Model>>> GetModelsAsync(int manufacturerId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of models for manufacturer with ID {manufacturerId} (page {pageNumber}, page size {pageSize})");

            List<Model> models = await Factory.Models
                                               .ListAsync(m => m.ManufacturerId == manufacturerId, pageNumber, pageSize)
                                               .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {models.Count} model(s)");

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
            LogMessage(Severity.Debug, $"Retrieving model with ID {id}");

            Model model = await Factory.Models.GetAsync(m => m.Id == id);

            if (model == null)
            {
                LogMessage(Severity.Debug, $"Model with ID {id} not found");
                return NotFound();
            }

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Model>> AddModelAsync([FromBody] Model template)
        {
            LogMessage(Severity.Debug, $"Adding model: {template}");
            Model model = await Factory.Models.AddAsync(template.Name, template.ManufacturerId);
            LogMessage(Severity.Debug, $"Added model: {model}");
            return model;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Model>> UpdateModelAsync([FromBody] Model template)
        {
            LogMessage(Severity.Debug, $"Updating model: {template}");

            Model model = await Factory.Models.UpdateAsync(
                template.Id,
                template.Name,
                template.ManufacturerId);

            LogMessage(Severity.Debug, $"Model updated: {model}");

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteModelAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting model: ID = {id}");
            await Factory.Models.DeleteAsync(id);
            return Ok();
        }
    }
}
