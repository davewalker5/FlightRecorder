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
        /// Return a JSON representation of the flight with the specified ID
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> FlightDetails(int flightId)
        {
            Flight flight = await _wizard.GetFlightAsync(flightId);
            return Json(flight);
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
            IActionResult result;
            string newAirline = (model.NewAirline ?? "").Trim();
            bool haveAirline = (model.AirlineId > 0) || !string.IsNullOrEmpty(newAirline);

            if (haveAirline && ModelState.IsValid && (model.Action == ControllerActions.ActionNextPage))
            {
                _wizard.CacheFlightDetailsModel(model);
                result = RedirectToAction("Index", "AircraftDetails");
            }
            else if (model.Action == ControllerActions.ActionPreviousPage)
            {
                _wizard.ClearCachedFlightDetailsModel();
                result = RedirectToAction("Index", "SightingDetails");
            }
            else
            {
                if (!haveAirline)
                {
                    model.AirlineErrorMessage = "You must select an airline or give a new airline name";
                }

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
