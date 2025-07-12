using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Db;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class SearchSightingsByAirlineController : Controller
    {
        private readonly IAirlineClient _airlines;
        private readonly ISightingsSearchClient _client;
        private readonly FlightRecorderApplicationSettings _settings;

        public SearchSightingsByAirlineController(IAirlineClient airlines, ISightingsSearchClient client, FlightRecorderApplicationSettings settings)
        {
            _airlines = airlines;
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by airline page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SightingSearchByAirlineViewModel model = new SightingSearchByAirlineViewModel
            {
                PageNumber = 1
            };
            List<Airline> airlines = await _airlines.GetAirlinesAsync();
            model.SetAirlines(airlines);
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the search
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SightingSearchByAirlineViewModel model)
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

                List<Sighting> sightings = await _client.GetSightingsByAirline(model.AirlineId, page, _settings.SearchPageSize);
                model.SetSightings(sightings, page, _settings.SearchPageSize);

                List<Airline> airlines = await _airlines.GetAirlinesAsync();
                model.SetAirlines(airlines);
            }

            return View(model);
        }
    }
}
