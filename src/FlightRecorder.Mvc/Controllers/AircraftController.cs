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
    public class AircraftController : Controller
    {
        private readonly ManufacturerClient _manufacturers;
        private readonly ModelClient _models;
        private readonly AircraftClient _aircraft;
        private readonly IMapper _mapper;

        public AircraftController(ManufacturerClient manufacturers, ModelClient models, AircraftClient aircraft, IMapper mapper)
        {
            _manufacturers = manufacturers;
            _models = models;
            _aircraft = aircraft;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the aircraft list page
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int manufacturerId = 0, int modelId = 0)
        {
            // Construct the model and assign the selected manufacturer and aircraft
            // model
            ListAircraftViewModel model = new ListAircraftViewModel();
            model.ManufacturerId = manufacturerId;
            model.ModelId = modelId;

            // Load the manufacturer list
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
            model.SetManufacturers(manufacturers);

            return View(model);
        }

        /// <summary>
        /// Return a list of models for the specified manufacturer
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Models(int manufacturerId)
        {
            ListAircraftViewModel model = new ListAircraftViewModel();
            List<Model> aircraftModels = await _models.GetModelsAsync(manufacturerId);
            model.SetModels(aircraftModels);
            return PartialView(model);
        }
    }
}
