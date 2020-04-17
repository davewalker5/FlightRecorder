using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly LocationClient _client;

        public LocationsController(LocationClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the locations list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Location> locations = await _client.GetLocationsAsync();
            return View(locations);
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
