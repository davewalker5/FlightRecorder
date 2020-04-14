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
    public class AircraftController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public AircraftController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("manufacturer/{manufacturerId}")]
        public async Task<ActionResult<List<Aircraft>>> GetAircraftByManufacturerAsync(int manufacturerId)
        {
            List<Aircraft> aircraft = await _factory.Aircraft
                                                    .ListAsync(a => a.Model.ManufacturerId == manufacturerId)
                                                    .ToListAsync();

            if (!aircraft.Any())
            {
                return NoContent();
            }

            return aircraft;
        }

        [HttpGet]
        [Route("model/{modelId}")]
        public async Task<ActionResult<List<Aircraft>>> GetAircraftByModelAsync(int modelId)
        {
            List<Aircraft> aircraft = await _factory.Aircraft
                                                    .ListAsync(a => a.ModelId == modelId)
                                                    .ToListAsync();

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
            Aircraft aircraft = await _factory.Aircraft
                                              .GetAsync(a => a.Registration == decodedRegistration);

            if (aircraft == null)
            {
                return NotFound();
            }

            // TODO : This logic should be in the business logic
            await _factory.Context.Entry(aircraft).Reference(a => a.Model).LoadAsync();
            await _factory.Context.Entry(aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();

            return aircraft;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Aircraft>> GetAircraftByIdAsync(int id)
        {
            Aircraft aircraft = await _factory.Aircraft.GetAsync(a => a.Id == id);

            if (aircraft == null)
            {
                return NotFound();
            }

            // TODO : This logic should be in the business logic
            await _factory.Context.Entry(aircraft).Reference(a => a.Model).LoadAsync();
            await _factory.Context.Entry(aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();

            return aircraft;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Aircraft>> UpdateAircraftAsync([FromBody] Aircraft template)
        {
            // TODO : Move this functionality to the Business Logic assembly
            template.Registration = template.Registration.ToUpper();
            template.SerialNumber = template.SerialNumber.ToUpper();

            Aircraft aircraft = await _factory.Aircraft
                        .GetAsync(a => (a.Registration == template.Registration) ||
                                       (  (a.Model.ManufacturerId == template.Model.ManufacturerId) &&
                                          (a.SerialNumber == template.SerialNumber)));

            if ((aircraft != null) && (aircraft.Id != template.Id))
            {
                return BadRequest();
            }

            aircraft = await _factory.Aircraft.GetAsync(m => m.Id == template.Id);
            if (aircraft == null)
            {
                return NotFound();
            }

            aircraft.Registration = template.Registration;
            aircraft.SerialNumber = template.SerialNumber;
            aircraft.ModelId = template.ModelId;
            await _factory.Context.SaveChangesAsync();
            await _factory.Context.Entry(aircraft).Reference(a => a.Model).LoadAsync();
            await _factory.Context.Entry(aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();

            return aircraft;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Aircraft>> CreateAircraftAsync([FromBody] Aircraft template)
        {
            // TODO : Should have create method taking model and manufacturer IDs
            Aircraft aircraft = await _factory.Aircraft
                                              .AddAsync(template.Registration,
                                                        template.SerialNumber,
                                                        template.Manufactured,
                                                        template.Model.Name,
                                                        template.Model.Manufacturer.Name);
            return aircraft;
        }
    }
}
