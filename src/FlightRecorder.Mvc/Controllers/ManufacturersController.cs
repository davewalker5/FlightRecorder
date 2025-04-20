using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class ManufacturersController : Controller
    {
        private readonly ManufacturerClient _client;
        private readonly IOptions<AppSettings> _settings;

        public ManufacturersController(ManufacturerClient client, IOptions<AppSettings> settings)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the manufacturers list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Manufacturer> manufacturers = await _client.GetManufacturersAsync(1, _settings.Value.SearchPageSize);
            var model = new ManufacturerListViewModel();
            model.SetManufacturers(manufacturers, 1, _settings.Value.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ManufacturerListViewModel model)
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
                var manufacturers = await _client.GetManufacturersAsync(page, _settings.Value.SearchPageSize);
                model.SetManufacturers(manufacturers, page, _settings.Value.SearchPageSize);
            }

            return View(model);
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
