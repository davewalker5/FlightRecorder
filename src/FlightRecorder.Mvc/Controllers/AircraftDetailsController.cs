using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    public class AircraftDetailsController : Controller
    {
        private AddSightingWizard _wizard;

        public AircraftDetailsController(AddSightingWizard wizard)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the aircraft details entry page
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AircraftDetailsViewModel model = await _wizard.GetAircraftDetailsModelAsync();
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
            List<Model> aircraftModels = await _wizard.GetModelsAsync(manufacturerId);
            model.SetModels(aircraftModels);
            return PartialView(model);
        }

        /// <summary>
        /// Handle POST events to cache aircraft details or move back to the flight
        /// details page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AircraftDetailsViewModel model)
        {
            IActionResult result = null;

            if (ModelState.IsValid && (model.Action == ControllerActions.ActionNextPage))
            {
                _wizard.CacheAircraftDetailsModel(model);
                // TODO : Redirect
            }
            else if (model.Action == ControllerActions.ActionPreviousPage)
            {
                _wizard.ClearCachedAircraftDetailsModel();
                result = RedirectToAction("Index", "FlightDetails");
            }
            else
            {
                // Set the list of available manufacturers
                List<Manufacturer> manufacturers = await _wizard.GetManufacturersAsync();
                model.SetManufacturers(manufacturers);

                // Load the models for the aircraft's manufacturer
                if (model.ManufacturerId > 0)
                {
                    List<Model> models = await _wizard.GetModelsAsync(model.ManufacturerId ?? 0);
                    model.SetModels(models);
                }

                result = View(model);
            }

            return result;
        }
    }
}
