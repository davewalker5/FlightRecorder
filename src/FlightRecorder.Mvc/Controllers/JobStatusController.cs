﻿using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class JobStatusController : Controller
    {
        private readonly ReportsClient _reportsClient;
        private readonly ExportClient _exportClient;
        private readonly IOptions<AppSettings> _settings;

        public JobStatusController(
            ReportsClient reportsClient,
            ExportClient exportsClient,
            IOptions<AppSettings> settings)
        {
            _reportsClient = reportsClient;
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
            JobStatusViewModel model = new JobStatusViewModel
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
        public async Task<IActionResult> Index(JobStatusViewModel model)
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

                // Get the date and time
                DateTime start = !string.IsNullOrEmpty(model.From) ? DateTime.Parse(model.From) : DateTime.MinValue;
                DateTime end = !string.IsNullOrEmpty(model.To) ? DateTime.Parse(model.To) : DateTime.MaxValue;

                // Retrieve the matching report records
                List<JobStatus> records = await _reportsClient.JobStatusAsync(start, end, page, _settings.Value.SearchPageSize);
                model.SetRecords(records, page, _settings.Value.SearchPageSize);
            }

            return View(model);
        }

        /// <summary>
        /// Request export of the report
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Export([FromBody] JobStatusViewModel model)
        {
            await _exportClient.ExportReport<JobStatus>(model);
            return Ok();
        }
    }
}