using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class ExportController : Controller
    {
        private readonly ExportClient _client;

        public ExportController(ExportClient client)
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
                var selectedReportType = model.ReportType ?? 0;
                await _client.ExportReports(selectedReportType, null, null, model.FileName);
                model.Message = $"Background export of the {model.ReportTypeNames[selectedReportType]} report to {model.FileName} has been submitted";
                model.Clear();
                ModelState.Clear();
            }

            return View(model);
        }
    }
}
