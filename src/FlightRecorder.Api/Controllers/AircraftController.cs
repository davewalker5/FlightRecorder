using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class AircraftController : FlightRecorderApiController
    {
        public AircraftController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("manufacturer/{manufacturerId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Aircraft>>> GetAircraftByManufacturerAsync(int manufacturerId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of aircraft for manufacturer with ID {manufacturerId} (page {pageNumber}, page size {pageSize})");

            List<Aircraft> aircraft = await Factory.Aircraft
                                                    .ListAsync(a => a.Model.ManufacturerId == manufacturerId, pageNumber, pageSize)
                                                    .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {aircraft.Count} aircraft");

            if (!aircraft.Any())
            {
                return NoContent();
            }

            return aircraft;
        }

        [HttpGet]
        [Route("model/{modelId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Aircraft>>> GetAircraftByModelAsync(int modelId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of aircraft for model with ID {modelId} (page {pageNumber}, page size {pageSize})");

            List<Aircraft> aircraft = await Factory.Aircraft
                                                    .ListAsync(a => a.ModelId == modelId, pageNumber, pageSize)
                                                    .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {aircraft.Count} aircraft");

            if (!aircraft.Any())
            {
                return NoContent();
            }

            return aircraft;
        }

        [HttpGet]
        [Route("registration/{registration}")]
        public async Task<ActionResult<Aircraft>> GetAircraftByRegistrationAsync(string registration)
        {
            string decodedRegistration = HttpUtility.UrlDecode(registration).ToUpper();
            LogMessage(Severity.Debug, $"Retrieving aircraft with registration {registration}");
            Aircraft aircraft = await Factory.Aircraft
                                              .GetAsync(a => a.Registration == decodedRegistration);

            if (aircraft == null)
            {
                LogMessage(Severity.Debug, $"Aircraft with registration {registration} not found");
                return NotFound();
            }

            LogMessage(Severity.Debug, $"Aircraft retrieved: {aircraft}");

            return aircraft;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Aircraft>> GetAircraftByIdAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving aircraft with ID {id}");
            Aircraft aircraft = await Factory.Aircraft.GetAsync(a => a.Id == id);

            if (aircraft == null)
            {
                LogMessage(Severity.Debug, $"Aircraft with ID {id} not found");
                return NotFound();
            }

            LogMessage(Severity.Debug, $"Aircraft retrieved: {aircraft}");

            return aircraft;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Aircraft>> UpdateAircraftAsync([FromBody] Aircraft template)
        {
            Aircraft aircraft = null;

            LogMessage(Severity.Debug, $"Updating aircraft: {template}");

            try
            {
                long? manufactured = (template.Manufactured > 0) ? template.Manufactured : null;
                aircraft = await Factory.Aircraft.UpdateAsync(
                    template.Id,
                    template.Registration,
                    template.SerialNumber,
                    manufactured,
                    template.ModelId);
            }
            catch (AircraftNotFoundException ex)
            {
                Logger.LogException(ex);
                return NotFound();
            }
            catch (AircraftExistsException ex)
            {
                Logger.LogException(ex);
                return BadRequest();
            }

            LogMessage(Severity.Debug, $"Aircraft updated: {aircraft}");
            return aircraft;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Aircraft>> CreateAircraftAsync([FromBody] Aircraft template)
        {
            LogMessage(Severity.Debug, $"Creating aircraft: {template}");

            // TODO : Should have create method taking model and manufacturer IDs
            long? manufactured = (template.Manufactured > 0) ? template.Manufactured : null;
            Aircraft aircraft = await Factory.Aircraft
                                                .AddAsync(template.Registration,
                                                          template.SerialNumber,
                                                          manufactured,
                                                          template.ModelId);
            return aircraft;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAircraftAsync(int id)
        {
            await Factory.Aircraft.DeleteAsync(id);
            return Ok();
        }
    }
}
