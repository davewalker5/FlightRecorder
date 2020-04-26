using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    public class FlightDetailsController : Controller
    {
        private AddSightingWizard _wizard;

        public FlightDetailsController(AddSightingWizard wizard)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the flight details entry page
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            FlightDetailsViewModel model = await _wizard.GetFlightDetailsModelAsync();
            return View(model);
        }

        /// <summary>
        /// Handle POST events to cache flight details or move back to the sighting
        /// details page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FlightDetailsViewModel model)
        {
            IActionResult result = null;

            if (ModelState.IsValid)
            {
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        _wizard.ClearCachedFlightDetailsModel();
                        result = RedirectToAction("Index", "SightingDetails");
                        break;
                    case ControllerActions.ActionNextPage:
                        _wizard.CacheFlightDetailsModel(model);
                        result = RedirectToAction("Index", "AircraftDetails");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                List<Flight> flights = await _wizard.GetFlightsAsync(model.FlightNumber);
                model.SetFlights(flights);

                List<Airline> airlines = await _wizard.GetAirlinesAsync();
                model.SetAirlines(airlines);

                result = View(model);
            }

            return result;
        }
    }
}
