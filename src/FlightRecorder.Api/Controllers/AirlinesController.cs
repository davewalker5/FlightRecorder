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
    public class AirlinesController : FlightRecorderApiController
    {
        public AirlinesController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airline>>> GetAirlinesAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of airlines (page {pageNumber}, page size {pageSize})");

            List<Airline> airlines = await Factory.Airlines
                                                   .ListAsync(null, pageNumber, pageSize)
                                                   .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {airlines.Count} airline(s)");

            if (!airlines.Any())
            {
                return NoContent();
            }

            return airlines;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Airline>> GetAirlineAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving airline with ID {id}");

            Airline airline = await Factory.Airlines
                                            .GetAsync(m => m.Id == id);

            if (airline == null)
            {
                LogMessage(Severity.Debug, $"Airline with ID {id} not found");
                return NotFound();
            }

            return airline;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Airline>> AddAirlineAsync([FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Creating airline: Name = {name}");
            var airline = await Factory.Airlines.AddAsync(name);
            LogMessage(Severity.Debug, $"Location added: {airline}");
            return airline;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Airline>> UpdateAirlineAsync(int id, [FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Updating airline: ID = {id}, Name = {name}");
            var airline = await Factory.Airlines.UpdateAsync(id, name);
            LogMessage(Severity.Debug, $"Airline updated: {airline}");
            return airline;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAirlineAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting airline: ID = {id}");
            await Factory.Airlines.DeleteAsync(id);
            return Ok();
        }
    }
}
