using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
    public class AirportsController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public AirportsController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airport>>> GetAirportsAsync(int pageNumber, int pageSize)
        {
            List<Airport> airports = await _factory.Airports
                                                 .ListAsync(null, pageNumber, pageSize)
                                                 .ToListAsync();

            if (!airports.Any())
            {
                return NoContent();
            }

            return airports;
        }

        [HttpGet]
        [Route("code/{code}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airport>>> GetAirportsByCodeAsync(string code, int pageNumber, int pageSize)
        {
            string decodedCode = HttpUtility.UrlDecode(code).ToUpper();
            List<Airport> airports = await _factory.Airports
                                                 .ListAsync(f => f.Code == decodedCode, pageNumber, pageSize)
                                                 .ToListAsync();

            if (!airports.Any())
            {
                return NoContent();
            }

            return airports;
        }

        [HttpGet]
        [Route("country/{countryId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airport>>> GetAirportsByCountryAsync(int countryId, int pageNumber, int pageSize)
        {
            List<Airport> airports = await _factory.Airports
                                                 .ListAsync(f => f.CountryId == countryId, pageNumber, pageSize)
                                                 .ToListAsync();

            if (!airports.Any())
            {
                return NoContent();
            }

            return airports;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Airport>> GetAirportByIdAsync(int id)
        {
            Airport airport = await _factory.Airports.GetAsync(f => f.Id == id);

            if (airport == null)
            {
                return NotFound();
            }

            // TODO : This logic should be in the business logic
            await _factory.Context.Entry(airport).Reference(f => f.Country).LoadAsync();

            return airport;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Airport>> UpdateAirportAsync([FromBody] Airport template)
        {
            // TODO This logic should be in the business logic
            Airport airport = await _factory.Airports.GetAsync(f => f.Id == template.Id);
            if (airport == null)
            {
                return NotFound();
            }

            airport.Code = template.Code.ToUpper();
            airport.Name = template.Name;
            airport.CountryId = template.CountryId;
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(airport).Reference(a => a.Country).LoadAsync();

            return airport;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Airport>> CreateAirportAsync([FromBody] Airport template)
        {
            // TODO : Should have a method taking the country ID
            Airport airport = await _factory.Airports
                                          .AddAsync(template.Code,
                                                    template.Name,
                                                    template.Country.Name);
            return airport;
        }
    }
}
