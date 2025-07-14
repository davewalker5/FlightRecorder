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

            // TODO : This logic should be in the business logic
            await Factory.Context.Entry(model).Reference(m => m.Manufacturer).LoadAsync();

            return model;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Model>> UpdateModelAsync([FromBody] Model template)
        {
            Model model;

            LogMessage(Severity.Debug, $"Updating model: {template}");

            try
            {
                model = await Factory.Models.UpdateAsync(
                    template.Id,
                    template.Name,
                    template.ManufacturerId);
            }
            catch (ModelNotFoundException ex)
            {
                Logger.LogException(ex);
                return NotFound();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return BadRequest();
            }

            LogMessage(Severity.Debug, $"Model updated: {model}");

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Model>> CreateModelAsync([FromBody] Model template)
        {
            LogMessage(Severity.Debug, $"Creating model: {template}");
            Model location = await Factory.Models.AddAsync(template.Name, template.ManufacturerId);
            return location;
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
