using AutoMapper;
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
    public class AirportsController : Controller
    {
        private readonly ICountriesClient _countries;
        private readonly IAirportsClient _airports;
        private readonly IMapper _mapper;
        private readonly FlightRecorderApplicationSettings _settings;

        public AirportsController(
            ICountriesClient countries,
            IAirportsClient airports,
            IMapper mapper,
            FlightRecorderApplicationSettings settings)
        {
            _countries = countries;
            _airports = airports;
            _mapper = mapper;
            _settings = settings;
        }

        /// <summary>
        /// Serve the airports list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var airports = await _airports.GetAirportsAsync(1, _settings.SearchPageSize);
            var model = new AirportListViewModel();
            model.SetAirports(airports, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AirportListViewModel model)
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

                // Retrieve the matching airport records
                var airports = await _airports.GetAirportsAsync(page, _settings.SearchPageSize);
                model.SetAirports(airports, page, _settings.SearchPageSize);
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new airprot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddAirportViewModel model = new AddAirportViewModel();
            List<Country> countries = await _countries.GetCountriesAsync(1, int.MaxValue);
            model.SetCountries(countries);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new airports
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddAirportViewModel model)
        {
            if (ModelState.IsValid)
            {
                string code = model.Code;
                string name = model.Name;
                await _airports.AddAirportAsync(model.Code, model.Name, model.CountryId);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Airport '{code} - {name}' added successfully";
            }

            List<Country> countries = await _countries.GetCountriesAsync(1, int.MaxValue);
            model.SetCountries(countries);

            return View(model);
        }

        /// <summary>
        /// Serve the airport editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Airport airport = await _airports.GetAirportByIdAsync(id);
            EditAirportViewModel model = _mapper.Map<EditAirportViewModel>(airport);
            List<Country> countries = await _countries.GetCountriesAsync(1, int.MaxValue);
            model.SetCountries(countries);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated airports
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAirportViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _airports.UpdateAirportAsync(model.Id, model.Code, model.Name, model.CountryId);
                result = RedirectToAction("Index", new
                {
                    countryId = 0,
                    number = model.Code,
                    code = "",
                    name = ""
                });
            }
            else
            {
                List<Country> countries = await _countries.GetCountriesAsync(1, int.MaxValue);
                model.SetCountries(countries);
                result = View(model);
            }

            return result;
        }
    }
}
