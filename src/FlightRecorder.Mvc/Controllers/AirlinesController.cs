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
    public class AirlinesController : Controller
    {
        private readonly AirlineClient _client;

        public AirlinesController(AirlineClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Serve the airlines list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Airline> airlines = await _client.GetAirlinesAsync();
            return View(airlines);
        }

        /// <summary>
        /// Serve the page to add a new airline
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddAirlineViewModel());
        }

        /// <summary>
        /// Handle POST events to save new airlines
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddAirlineViewModel model)
        {
            if (ModelState.IsValid)
            {
                Airline airline = await _client.AddAirlineAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Airline '{airline.Name}' added successfully";
            }

            return View(model);
        }

        /// <summary>
        /// Serve the airline editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Airline model = await _client.GetAirlineAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated airlines
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Airline model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateAirlineAsync(model.Id, model.Name);
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
