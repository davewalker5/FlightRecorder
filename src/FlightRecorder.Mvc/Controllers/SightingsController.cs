using System.Threading.Tasks;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    public class SightingsController : Controller
    {
        /// <summary>
        /// Serve the page to add a new sighting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddSightingViewModel model = new AddSightingViewModel();
            return View();
        }
    }
}
