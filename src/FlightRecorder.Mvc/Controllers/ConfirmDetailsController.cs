using System.Threading.Tasks;
using FlightRecorder.Mvc.Models;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
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
            ConfirmDetailsViewModel model = await _wizard.GetConfirmDetailsModelAsync();
            return View(model);
        }
    }
}
