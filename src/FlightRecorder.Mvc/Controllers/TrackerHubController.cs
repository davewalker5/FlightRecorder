using FlightRecorder.Entities.Config;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class TrackerHubController : FlightRecorderControllerBase
    {
        private readonly FlightRecorderApplicationSettings _settings;

        public TrackerHubController(
            FlightRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<MyFlightsController> logger) : base(renderer, logger)
        {
            _settings = settings;
        }
        
        /// <summary>
        /// Serve the aircraft tracking page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            // Pass HubUrl to the view (avoids hardcoding)
            ViewData["TrackerHubUrl"] = _settings.TrackerHubUrl;
            return View();
        }
    }
}