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
    public class ModelsController : FlightRecorderControllerBase
    {
        private readonly IManufacturerClient _manufacturers;
        private readonly IModelClient _models;
        private readonly IMapper _mapper;

        public ModelsController(
            IManufacturerClient manufacturers,
            IModelClient models,
            IMapper mapper,
            IPartialViewToStringRenderer renderer,
            ILogger<ModelsController> logger) : base (renderer, logger)
        {
            _manufacturers = manufacturers;
            _models = models;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int manufacturerId = 0)
        {
            // Construct the model and assign the selected manufacturer
            ListModelsViewModel model = new ListModelsViewModel();
            model.ManufacturerId = manufacturerId;

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
        public async Task<IActionResult> List(int manufacturerId)
        {
            List<Model> models = await _models.GetModelsAsync(manufacturerId);
            return PartialView(models);
        }

        /// <summary>
        /// Serve the page to add a new model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
            AddModelViewModel model = new AddModelViewModel();
            model.SetManufacturers(manufacturers);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new models
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddModelViewModel model)
        {
            if (ModelState.IsValid)
            {
                Model aircraftModel = await _models.AddModelAsync(model.Name, model.ManufacturerId);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Model '{aircraftModel.Name}' added successfully";
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
        /// Serve the model editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Model aircraftModel = await _models.GetModelAsync(id);
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
            EditModelViewModel model = _mapper.Map<EditModelViewModel>(aircraftModel);
            model.SetManufacturers(manufacturers);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated models
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditModelViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _models.UpdateModelAsync(model.Id, model.ManufacturerId, model.Name);
                result = RedirectToAction("Index", new { manufacturerId = model.ManufacturerId });
            }
            else
            {
                List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync(1, int.MaxValue);
                model.SetManufacturers(manufacturers);
                result = View(model);
            }

            return result;
        }
    }
}
