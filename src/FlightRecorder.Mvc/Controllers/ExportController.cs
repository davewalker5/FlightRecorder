using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Reporting;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class ExportController : FlightRecorderControllerBase
    {
        private readonly IExportClient _client;

        public ExportController(
            IExportClient client,
            IPartialViewToStringRenderer renderer,
            ILogger<ExportController> logger) : base (renderer, logger)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the sightings export page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Sightings()
        {
            ExportViewModel model = new ExportViewModel
            {
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events to trigger sightings exports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Sightings(ExportViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _client.ExportSightings(model.FileName);
                model.Message = $"Background export of sightings to {model.FileName} has been submitted";
                model.Clear();
                ModelState.Clear();
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the airports export page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Airports()
        {
            ExportViewModel model = new ExportViewModel
            {
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events to export the current airports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Airports(ExportViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _client.ExportAirports(model.FileName);
                model.Message = $"Background export of airports to {model.FileName} has been submitted";
                model.Clear();
                ModelState.Clear();
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the report export page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Reports()
        {
            ExportReportViewModel model = new ExportReportViewModel
            {
            };

            return View(model);
        }

        /// <summary>
        /// Handle POST events to export reports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Reports(ExportReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var selectedReportType = (ReportType)(model.ReportType ?? 0);
                var reportDisplayName = ReportDefinitions.Definitions.First(x => x.ReportType == selectedReportType).DisplayName;
                await _client.ExportReport(selectedReportType, null, null, model.FileName);
                model.Message = $"Background export of the {reportDisplayName} report to {model.FileName} has been submitted";
                model.Clear();
                ModelState.Clear();
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
    }
}
