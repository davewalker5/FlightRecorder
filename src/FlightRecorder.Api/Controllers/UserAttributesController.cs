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
    public class UserAttributesController : Controller
    {
        private readonly FlightRecorderFactory _factory;
        private readonly IFlightRecorderLogger _logger;

        public UserAttributesController(FlightRecorderFactory factory, IFlightRecorderLogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        [HttpGet]
        [Route("id/{userId:int}")]
        public async Task<ActionResult<List<UserAttributeValue>>> GetUserAttributeValuesAsync(int userId)
        {
            _logger.LogMessage(Severity.Debug, $"Retrieving user attributes for user with ID {userId}");

            var user = await _factory.Users.GetUserAsync(userId);

            if (user == null)
            { 
                _logger.LogMessage(Severity.Error, $"User with ID {userId} not found");
                return NotFound();
            }

            if ((user.Attributes == null) || (user.Attributes.Count == 0))
            {
                _logger.LogMessage(Severity.Debug, $"No user attributes found for user with ID {userId}");
                return NoContent();
            }

            _logger.LogMessage(Severity.Debug, $"Returning {user.Attributes.Count} attribute(s) for user with ID {userId}");
            return user.Attributes.ToList();
        }

        [HttpGet]
        [Route("name/{userName}")]
        public async Task<ActionResult<List<UserAttributeValue>>> GetUserAttributeValuesAsync(string userName)
        {
            _logger.LogMessage(Severity.Debug, $"Retrieving user attributes for user {userName}");
            var user = await _factory.Users.GetUserAsync(userName);

            if (user == null)
            {
                _logger.LogMessage(Severity.Error, $"User {userName} not found");
                return NotFound();
            }

            if ((user.Attributes == null) || (user.Attributes.Count == 0))
            {
                _logger.LogMessage(Severity.Debug, $"No user attributes found for user {userName}");
                return NoContent();
            }

            _logger.LogMessage(Severity.Debug, $"Returning {user.Attributes.Count} attribute(s) for user {userName}");
            return user.Attributes.ToList();
        }
    }
}
