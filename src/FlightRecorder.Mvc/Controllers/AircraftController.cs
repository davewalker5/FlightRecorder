using AutoMapper;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Db;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class AircraftController : FlightRecorderControllerBase
    {
        private readonly IManufacturerClient _manufacturers;
        private readonly IModelClient _models;
        private readonly IAircraftClient _aircraft;
        private readonly IMapper _mapper;

        public AircraftController(
            IManufacturerClient manufacturers,
            IModelClient models,
            IAircraftClient aircraft,
            IMapper mapper,
            IPartialViewToStringRenderer renderer,
            ILogger<AircraftController> logger) : base (renderer, logger)
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
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
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
        /// Serve the page to add an aircraft
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddAircraftViewModel viewModel = new AddAircraftViewModel();
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
            viewModel.SetManufacturers(manufacturers);
            return View(viewModel);
        }

        /// <summary>
        /// Handle POST events to save new aircraft
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddAircraftViewModel model)
        {
            if (ModelState.IsValid)
            {
                int? manufactured = (model.Age != null) ? DateTime.Now.Year - model.Age : null;
                Aircraft aircraft = await _aircraft.AddAircraftAsync(model.Registration, model.SerialNumber, manufactured, model.ModelId);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Aircraft '{aircraft.Registration}' added successfully";
            }
            else
            {
                LogModelState();
            }

            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
            model.SetManufacturers(manufacturers);

            return View(model);
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
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
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
                int? manufactured = (viewModel.Age != null) ? DateTime.Now.Year - viewModel.Age : null;
                await _aircraft.UpdateAircraftAsync(viewModel.Id, viewModel.Registration, viewModel.SerialNumber, manufactured, viewModel.ModelId);
                result = RedirectToAction("Index", new { manufacturerId = viewModel.ManufacturerId, modelId = viewModel.ModelId });
            }
            else
            {
                LogModelState();
                List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
                viewModel.SetManufacturers(manufacturers);
                result = View(viewModel);
            }

            return result;
        }
    }
}
