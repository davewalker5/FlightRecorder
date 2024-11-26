using System.Collections.Generic;
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
    public class SearchSightingsByAircraftController : Controller
    {
        private readonly SightingsSearchClient _client;
        private readonly IOptions<AppSettings> _settings;

        private readonly AircraftClient _aircraft;

        public SearchSightingsByAircraftController(AircraftClient aircraft, SightingsSearchClient client, IOptions<AppSettings> settings)
        {
            _aircraft = aircraft;
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by aircraft page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = new SightingSearchByAircraftViewModel
            {
                PageNumber = 1
            };
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SightingSearchByAircraftViewModel model)
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

                List<Sighting> sightings = null;

                try
                {
                    // Retrieve the aircraft with the specified registration number
                    // then, if we have a valid aircraft, retrieve its sightings
                    Aircraft aircraft = await _aircraft.GetAircraftByRegistrationAsync(model.Registration);
                    if (aircraft != null)
                    {
                        sightings = await _client.GetSightingsByAircraft(aircraft.Id, page, _settings.Value.SearchPageSize);
                    }
                }
                catch
                {
                }

                // Expose the sightings to the View
                model.SetSightings(sightings, page, _settings.Value.SearchPageSize);
            }

            return View(model);
        }
    }
}
