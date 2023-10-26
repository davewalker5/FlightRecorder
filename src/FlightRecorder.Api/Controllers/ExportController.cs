using FlightRecorder.Api.Entities;
using FlightRecorder.BusinessLogic.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ExportController : Controller
    {
        private readonly IBackgroundQueue<ExportWorkItem> _queue;

        public ExportController(IBackgroundQueue<ExportWorkItem> queue)
        {
            _queue = queue;
        }

        [HttpPost]
        [Route("sightings")]
        public IActionResult ExportSightings([FromBody] ExportWorkItem item)
        {
            // Set the job name used in the job status record
            item.JobName = "Sightings Export";

            // Queue the work item
            _queue.Enqueue(item);
            return Accepted();
        }
    }
}
