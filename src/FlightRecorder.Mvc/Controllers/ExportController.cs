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
        /// Serve the export settings page
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
        /// Handle
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Sightings(ExportViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fileName = model.FileName;
                await _client.ExportSightings(fileName);
                model.Message = $"Background export of sightings to {fileName} has been submitted";
                model.FileName = null;
                ModelState.Clear();
            }

            return View(model);
        }
    }
}
