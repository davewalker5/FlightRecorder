using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DroneFlightLog.Mvc.Controllers
{
    [Authorize]
    public class ModelsController : Controller
    {
        private readonly ManufacturerClient _manufacturers;
        private readonly ModelClient _models;
        private readonly IMapper _mapper;

        public ModelsController(ManufacturerClient manufacturers, ModelClient models, IMapper mapper)
        {
            _manufacturers = manufacturers;
            _models = models;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Manufacturer> manufacturers = await _manufacturers.GetManufacturersAsync();
            ListModelsViewModel model = new ListModelsViewModel();
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
    }
}
