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
    public class SearchSightingsByAirlineController : Controller
    {
        private readonly AirlineClient _airlines;
        private readonly SightingsSearchClient _client;
        private readonly IOptions<AppSettings> _settings;

        public SearchSightingsByAirlineController(AirlineClient airlines, SightingsSearchClient client, IOptions<AppSettings> settings)
        {
            _airlines = airlines;
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty search sightings by route page
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

                List<Sighting> sightings = await _client.GetSightingsByAirline(model.AirlineId, page, _settings.Value.SearchPageSize);
                model.SetSightings(sightings, page, _settings.Value.SearchPageSize);

                List<Airline> airlines = await _airlines.GetAirlinesAsync();
                model.SetAirlines(airlines);
            }

            return View(model);
        }
    }
}
