using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data.Migrations;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public CountriesController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Country>>> GetCountriesAsync(int pageNumber, int pageSize)
        {
            List<Country> countries = await _factory.Countries
                                                   .ListAsync(null, pageNumber, pageSize)
                                                   .ToListAsync();

            if (!countries.Any())
            {
                return NoContent();
            }

            return countries;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Country>> GetCountryAsync(int id)
        {
            Country country = await _factory.Countries
                                            .GetAsync(m => m.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return country;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Country>> UpdateCountryAsync(int id, [FromBody] string name)
        {
            // TODO : Move this functionality to the Business Logic assembly
            Country country = await _factory.Countries
                                            .GetAsync(m => m.Name == name);
            if (country != null)
            {
                return BadRequest();
            }

            country = await _factory.Countries
                                    .GetAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            country.Name = name;
            await _factory.Context.SaveChangesAsync();

            return country;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Country>> CreateCountryAsync([FromBody] string name)
        {
            Country country = await _factory.Countries.AddAsync(name);
            return country;
        }
    }
}
