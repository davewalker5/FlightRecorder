using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace FlightRecorder.Mvc.Controllers
{
    public class ManufacturerStatisticsController : Controller
    {
        private readonly ReportsClient _client;
        private readonly IOptions<AppSettings> _settings;

        public ManufacturerStatisticsController(ReportsClient reports, IOptions<AppSettings> settings)
        {
            _client = reports;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty report page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            ManufacturerStatisticsViewModel model = new ManufacturerStatisticsViewModel
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
        public async Task<IActionResult> Index(ManufacturerStatisticsViewModel model)
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

                DateTime start = !string.IsNullOrEmpty(model.From) ? DateTime.Parse(model.From) : DateTime.MinValue;
                DateTime end = !string.IsNullOrEmpty(model.To) ? DateTime.Parse(model.To) : DateTime.MaxValue;

                List<ManufacturerStatistics> records = await _client.ManufacturerStatisticsAsync(start, end, page, _settings.Value.SearchPageSize);
                model.SetRecords(records, page, _settings.Value.SearchPageSize);
            }

            return View(model);
        }
    }
}
