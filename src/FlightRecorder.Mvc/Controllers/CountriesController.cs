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
    public class CountriesController : Controller
    {
        private readonly CountriesClient _client;

        public CountriesController(CountriesClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the country list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Country> countries = await _client.GetCountriesAsync();
            return View(countries);
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
