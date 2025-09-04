using FlightRecorder.Entities.Db;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class FlightDetailsController : FlightRecorderControllerBase
    {
        private AddSightingWizard _wizard;

        public FlightDetailsController(
            AddSightingWizard wizard,
            IPartialViewToStringRenderer renderer,
            ILogger<FlightDetailsController> logger) : base (renderer, logger)
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
            FlightDetailsViewModel model = await _wizard.GetFlightDetailsModelAsync(User.Identity.Name);
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
                _wizard.CacheFlightDetailsModel(model, User.Identity.Name);
                result = RedirectToAction("Index", "AircraftDetails");
            }
            else if (model.Action == ControllerActions.ActionPreviousPage)
            {
                _wizard.ClearCachedFlightDetailsModel(User.Identity.Name);
                long? sightingId = _wizard.GetCurrentSightingId(User.Identity.Name);
                result = RedirectToAction("Index", "SightingDetails", new { Id = sightingId });
            }
            else
            {
                if (!haveAirline)
                {
                    model.AirlineErrorMessage = "You must select an airline or give a new airline name";
                }

                LogModelState();

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
