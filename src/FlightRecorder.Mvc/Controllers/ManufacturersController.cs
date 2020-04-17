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
    public class ManufacturersController : Controller
    {
        private readonly ManufacturerClient _client;

        public ManufacturersController(ManufacturerClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the manufacturers list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Manufacturer> manufacturers = await _client.GetManufacturersAsync();
            return View(manufacturers);
        }

        /// <summary>
        /// Serve the page to add a new manufacturer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddManufacturerViewModel());
        }

        /// <summary>
        /// Handle POST events to save new manufacturers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddManufacturerViewModel model)
        {
            if (ModelState.IsValid)
            {
                Manufacturer manufacturer = await _client.AddManufacturerAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Manufacturer '{manufacturer.Name}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the manufacturer editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Manufacturer model = await _client.GetManufacturerAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated manufacturers
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Manufacturer model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateManufacturerAsync(model.Id, model.Name);
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
