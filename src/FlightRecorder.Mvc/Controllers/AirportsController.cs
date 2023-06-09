using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class AirportsController : Controller
    {
        private readonly CountriesClient _countries;
        private readonly AirportsClient _airports;
        private readonly IMapper _mapper;

        public AirportsController(CountriesClient countries, AirportsClient airports, IMapper mapper)
        {
            _countries = countries;
            _airports = airports;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the airports list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Airport> airports = await _airports.GetAirportsAsync();
            return View(airports);
        }

        /// <summary>
        /// Serve the page to add a new airprot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddAirportViewModel model = new AddAirportViewModel();
            List<Country> countries = await _countries.GetCountriesAsync();
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

            List<Country> countries = await _countries.GetCountriesAsync();
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
            List<Country> countries = await _countries.GetCountriesAsync();
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
                List<Country> countries = await _countries.GetCountriesAsync();
                model.SetCountries(countries);
                result = View(model);
            }

            return result;
        }
    }
}
