using FlightRecorder.Client.Interfaces;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class SightingStatisticsController : FlightRecorderControllerBase
    {
        private readonly IReportsClient _reportsClient;

        public SightingStatisticsController(
            IReportsClient iReportsClient,
            IPartialViewToStringRenderer renderer,
            ILogger<SightingStatisticsController> logger) : base(renderer, logger)
        {
            _reportsClient = iReportsClient;
        }

        /// <summary>
        /// Retrieve and serve the report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var report = await _reportsClient.SightingStatisticsAsync();
            return View(report);
        }
    }
}
