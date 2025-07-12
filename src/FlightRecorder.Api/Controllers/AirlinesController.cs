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

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Airline>> UpdateAirlineAsync(int id, [FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Updating airline: ID = {id}, Name = {name}");

            // TODO : Move this functionality to the Business Logic assembly
            Airline airline = await Factory.Airlines
                                            .GetAsync(m => m.Name == name);
            if (airline != null)
            {
                return BadRequest();
            }

            airline = await Factory.Airlines
                                    .GetAsync(m => m.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            airline.Name = name;
            await Factory.Context.SaveChangesAsync();

            return airline;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Airline>> CreateAirlineAsync([FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Creating airline: Name = {name}");
            Airline airline = await Factory.Airlines.AddAsync(name);
            return airline;
        }
    }
}
