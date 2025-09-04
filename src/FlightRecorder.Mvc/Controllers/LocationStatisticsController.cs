using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Reporting;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class LocationStatisticsController : FlightRecorderControllerBase
    {
        private readonly IReportsClient _reportsClient;
        private readonly IExportClient _exportClient;
        private readonly FlightRecorderApplicationSettings _settings;

        public LocationStatisticsController(
            IReportsClient iReportsClient,
            IExportClient exportsClient,
            FlightRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<LocationStatisticsController> logger) : base (renderer, logger)
        {
            _reportsClient = iReportsClient;
            _exportClient = exportsClient;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty report page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            LocationStatisticsViewModel model = new LocationStatisticsViewModel
            {
                PageNumber = 1
            };
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the report generation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LocationStatisticsViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    case ControllerActions.ActionSearch:
                        page = 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                DateTime start = model.From ?? DateTime.MinValue;
                DateTime end = model.To ?? DateTime.MaxValue;

                List<LocationStatistics> records = await _reportsClient.LocationStatisticsAsync(start, end, page, _settings.SearchPageSize);
                model.SetRecords(records, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Request export of the report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Export([FromBody] LocationStatisticsViewModel model)
        {
            await _exportClient.ExportReport<LocationStatistics>(model.From, model.To);
            return Ok();
        }
    }
}
