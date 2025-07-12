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
    public class AirportsController : FlightRecorderApiController
    {
        public AirportsController(FlightRecorderFactory Factory, IFlightRecorderLogger logger) : base(Factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airport>>> GetAirportsAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of airports (page {pageNumber}, page size {pageSize})");

            List<Airport> airports = await Factory.Airports
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
            LogMessage(Severity.Debug, $"Retrieving list of airports with code {code} (page {pageNumber}, page size {pageSize})");

            string decodedCode = HttpUtility.UrlDecode(code).ToUpper();
            List<Airport> airports = await Factory.Airports
                                                 .ListAsync(f => f.Code == decodedCode, pageNumber, pageSize)
                                                 .ToListAsync();

            if (!airports.Any())
            {
                LogMessage(Severity.Debug, $"No matching airports found");
                return NoContent();
            }

            return airports;
        }

        [HttpGet]
        [Route("country/{countryId}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Airport>>> GetAirportsByCountryAsync(int countryId, int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of airports for country ID {countryId} (page {pageNumber}, page size {pageSize})");

            List<Airport> airports = await Factory.Airports
                                                 .ListAsync(f => f.CountryId == countryId, pageNumber, pageSize)
                                                 .ToListAsync();

            if (!airports.Any())
            {
                LogMessage(Severity.Debug, $"No matching airports found");
                return NoContent();
            }

            return airports;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Airport>> GetAirportByIdAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving airport with ID {id}");

            Airport airport = await Factory.Airports.GetAsync(f => f.Id == id);

            if (airport == null)
            {
                LogMessage(Severity.Debug, $"Airport with ID {id} not found");
                return NotFound();
            }

            return airport;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Airport>> UpdateAirportAsync([FromBody] Airport template)
        {
            Airport airport = null;

            LogMessage(Severity.Debug, $"Updating airport: {template}");

            try
            {
                airport = await Factory.Airports.UpdateAsync(
                    template.Id,
                    template.Code,
                    template.Name,
                    template.CountryId);
            }
            catch (AirportNotFoundException ex)
            {
                Logger.LogException(ex);
                return NotFound();
            }
            catch (AirportExistsException ex)
            {
                Logger.LogException(ex);
                return BadRequest();
            }

            LogMessage(Severity.Debug, $"Airport updated: {airport}");
            return airport;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Airport>> CreateAirportAsync([FromBody] Airport template)
        {
            LogMessage(Severity.Debug, $"Creating airport: {template}");
            Airport airport = await Factory.Airports
                                          .AddAsync(template.Code,
                                                    template.Name,
                                                    template.CountryId);
            return airport;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAirportAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting airport: ID = {id}");
            await Factory.Airports.DeleteAsync(id);
            return Ok();
        }
    }
}
