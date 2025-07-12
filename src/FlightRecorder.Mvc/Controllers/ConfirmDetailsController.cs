using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class ConfirmDetailsController : Controller
    {
        private AddSightingWizard _wizard;

        public ConfirmDetailsController(AddSightingWizard wizard)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the confirm details page
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ConfirmDetailsViewModel model = await _wizard.GetConfirmDetailsModelAsync(User.Identity.Name);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to cache aircraft details or move back to the flight
        /// details page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConfirmDetailsViewModel viewModel)
        {
            IActionResult result = null;

            switch (viewModel.Action)
            {
                case ControllerActions.ActionNextPage:
                    await _wizard.CreateSighting(User.Identity.Name);
                    result = RedirectToAction("Index", "SightingDetails");
                    break;
                case ControllerActions.ActionPreviousPage:
                    result = RedirectToAction("Index", "AircraftDetails");
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
