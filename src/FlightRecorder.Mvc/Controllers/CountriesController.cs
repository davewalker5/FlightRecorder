using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class CountriesController : Controller
    {
        private readonly CountriesClient _client;
        private readonly IOptions<AppSettings> _settings;

        public CountriesController(CountriesClient client, IOptions<AppSettings> settings)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the country list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var countries = await _client.GetCountriesAsync(1, _settings.Value.SearchPageSize);
            var model = new CountryListViewModel();
            model.SetCountries(countries, 1, _settings.Value.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CountryListViewModel model)
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

                // Retrieve the matching country records
                var countries = await _client.GetCountriesAsync(page, _settings.Value.SearchPageSize);
                model.SetCountries(countries, page, _settings.Value.SearchPageSize);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new country
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddCountryViewModel());
        }

        /// <summary>
        /// Handle POST events to save new countries
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddCountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Country country = await _client.AddCountryAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Country '{country.Name}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the country editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Country model = await _client.GetCountryAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated countries
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateCountryAsync(model.Id, model.Name);
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
