using System;
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

        /// <summary>
        /// Return a list of aircraft for the specified model
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListByModel(int modelId)
        {
            List<Aircraft> aircraft = await _aircraft.GetAircraftByModelAsync(modelId);
            return PartialView("List", aircraft);
        }

        /// <summary>
        /// Return a list of aircraft for the specified registration number
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListByRegistration(string registration)
        {
            List<Aircraft> viewModel = null;
            Aircraft aircraft = await _aircraft.GetAircraftByRegistrationAsync(registration);
            if (aircraft != null)
            {
                viewModel = new List<Aircraft> { aircraft };
            }
            return PartialView("List", viewModel);
        }

        /// <summary>
        /// Serve the page to edit an aircraft
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Aircraft aircraft = await _aircraft.GetAircraftByIdAsync(id);
            EditAircraftViewModel viewModel = _mapper.Map<EditAircraftViewModel>(aircraft);
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
            viewModel.SetManufacturers(manufacturers);
            return View(viewModel);
        }

        /// <summary>
        /// Handle POST events to save updated models
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAircraftViewModel viewModel)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                int manufactured = DateTime.Now.Year - viewModel.Age;
                await _aircraft.UpdateAircraftAsync(viewModel.Id, viewModel.Registration, viewModel.SerialNumber, manufactured, viewModel.ManufacturerId, viewModel.ModelId);
                result = RedirectToAction("Index", new { manufacturerId = viewModel.ManufacturerId, modelId = viewModel.ModelId });
            }
            else
            {
                List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
                viewModel.SetManufacturers(manufacturers);
                result = View(viewModel);
            }

            return result;
        }
    }
}
