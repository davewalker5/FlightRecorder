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
    public class UserAttributesController : Controller
    {
        private readonly FlightRecorderFactory _factory;

        public UserAttributesController(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Route("id/{userId:int}")]
        public async Task<ActionResult<List<UserAttributeValue>>> GetUserAttributeValuesAsync(int userId)
        {
            var user = await _factory.Users.GetUserAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if ((user.Attributes == null) || (user.Attributes.Count == 0))
            {
                return NoContent();
            }

            return user.Attributes.ToList();
        }

        [HttpGet]
        [Route("name/{userName}")]
        public async Task<ActionResult<List<UserAttributeValue>>> GetUserAttributeValuesAsync(string userName)
        {
            var user = await _factory.Users.GetUserAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            if ((user.Attributes == null) || (user.Attributes.Count == 0))
            {
                return NoContent();
            }

            return user.Attributes.ToList();
        }
    }
}
