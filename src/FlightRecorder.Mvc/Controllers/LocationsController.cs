﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly LocationClient _client;
        private readonly IOptions<AppSettings> _settings;

        public LocationsController(LocationClient client, IOptions<AppSettings> settings)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the locations list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Location> locations = await _client.GetLocationsAsync(1, _settings.Value.SearchPageSize);
            var model = new LocationListViewModel();
            model.SetLocations(locations, 1, _settings.Value.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LocationListViewModel model)
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
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching airport records
                var locations = await _client.GetLocationsAsync(page, _settings.Value.SearchPageSize);
                model.SetLocations(locations, page, _settings.Value.SearchPageSize);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddLocationViewModel());
        }

        /// <summary>
        /// Handle POST events to save new locations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddLocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                Location location = await _client.AddLocationAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Location '{location.Name}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the location editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Location model = await _client.GetLocationAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated locations
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Location model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateLocationAsync(model.Id, model.Name);
                result = RedirectToAction("Index");
            }
            else
            {
                result = View(model);
            }

            return result;
        }
    }
}
