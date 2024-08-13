using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class SightingStatisticsController : Controller
    {
        private readonly ReportsClient _reportsClient;
        private readonly IOptions<AppSettings> _settings;

        public SightingStatisticsController(
            ReportsClient reportsClient,
            IOptions<AppSettings> settings)
        {
            _reportsClient = reportsClient;
            _settings = settings;
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
