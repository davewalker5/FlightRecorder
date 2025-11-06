using FlightRecorder.Entities.Config;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class TrackerHubController : FlightRecorderControllerBase
    {
        public TrackerHubController(
            IPartialViewToStringRenderer renderer,
            ILogger<MyFlightsController> logger) : base(renderer, logger)
        {
        }
        
        /// <summary>
        /// Serve the aircraft tracking page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            // Get the value of the SignalR Hub URL from the environment and pass it to the view
            ViewData["TrackerHubUrl"] = Environment.GetEnvironmentVariable("ADSB_TRACKER_HUB_URL");
            return View();
        }
    }
}