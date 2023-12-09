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
    public class AirlinesController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public AirlinesController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airline>>> GetAirlinesAsync(int pageNumber, int pageSize)
        {
            List<Airline> airlines = await _factory.Airlines
                                                   .ListAsync(null, pageNumber, pageSize)
                                                   .ToListAsync();

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
            Airline airline = await _factory.Airlines
                                            .GetAsync(m => m.Id == id);

            if (airline == null)
            {
                return NotFound();
            }

            return airline;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Airline>> UpdateAirlineAsync(int id, [FromBody] string name)
        {
            // TODO : Move this functionality to the Business Logic assembly
            Airline airline = await _factory.Airlines
                                            .GetAsync(m => m.Name == name);
            if (airline != null)
            {
                return BadRequest();
            }

            airline = await _factory.Airlines
                                    .GetAsync(m => m.Id == id);
            if (airline == null)
            {
                return NotFound();
            }

            airline.Name = name;
            await _factory.Context.SaveChangesAsync();

            return airline;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Airline>> CreateAirlineAsync([FromBody] string name)
        {
            Airline airline = await _factory.Airlines.AddAsync(name);
            return airline;
        }
    }
}
