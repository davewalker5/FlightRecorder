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
    public class CountriesController : FlightRecorderApiController
    {
        public CountriesController(FlightRecorderFactory factory, IFlightRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Country>>> GetCountriesAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of countries (page {pageNumber}, page size {pageSize})");

            List<Country> countries = await Factory.Countries
                                                   .ListAsync(null, pageNumber, pageSize)
                                                   .ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {countries.Count} countries");

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
            LogMessage(Severity.Debug, $"Retrieving country with ID {id}");

            Country country = await Factory.Countries
                                            .GetAsync(m => m.Id == id);

            if (country == null)
            {
                LogMessage(Severity.Debug, $"Country with ID {id} not found");
                return NotFound();
            }

            return country;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Country>> AddCountryAsync([FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Adding country: Name = {name}");
            Country country = await Factory.Countries.AddAsync(name);
            LogMessage(Severity.Debug, $"Added country: {country}");
            return country;
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Country>> UpdateCountryAsync(int id, [FromBody] string name)
        {
            LogMessage(Severity.Debug, $"Updating country: ID = {id}, Name = {name}");
            Country country = await Factory.Countries.UpdateAsync(id, name);
            LogMessage(Severity.Debug, $"Country updated: {country}");
            return country;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCountryAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting country: ID = {id}");
            await Factory.Countries.DeleteAsync(id);
            return Ok();
        }
    }
}
