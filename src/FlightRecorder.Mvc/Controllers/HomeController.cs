using System.Diagnostics;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class HomeController : FlightRecorderControllerBase
    {
        private AddSightingWizard _wizard;

        public HomeController(AddSightingWizard wizard)
        {
            _wizard = wizard;
        }

        /// <summary>
        /// Serve the models list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            // If we hit the home page via this action method, reset the add sighting wizard
            // as we're starting from scratch with a new entry
            _wizard.Reset(User.Identity.Name);
            return RedirectToAction("Index", "SightingDetails");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Construct the error view model
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = exceptionFeature != null ? exceptionFeature.Error.Message : ""
            };

            return View(model);
        }
    }
}
