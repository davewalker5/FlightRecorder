using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class SightingDetailsController : Controller
    {
        private AddSightingWizard _wizard;

        public SightingDetailsController(AddSightingWizard wizard)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the sighting details entry page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SightingDetailsViewModel model = await _wizard.GetSightingDetailsModelAsync();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to cache sighting details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SightingDetailsViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                _wizard.CacheSightingDetailsModel(model);
                result = RedirectToAction("Index", "FlightDetails", new { number = model.FlightNumber });
            }
            else
            {
                List<Location> locations = await _wizard.GetLocationsAsync();
                model.SetLocations(locations);
                result = View(model);
            }

            return result;
        }
    }
}
