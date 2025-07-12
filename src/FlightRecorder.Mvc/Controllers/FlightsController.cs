using AutoMapper;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Entities.Db;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class FlightsController : Controller
    {
        private readonly IAirlineClient _airlines;
        private readonly IFlightClient _flights;
        private readonly IMapper _mapper;

        public FlightsController(IAirlineClient airlines, IFlightClient flights, IMapper mapper)
        {
            _airlines = airlines;
            _flights = flights;
            _mapper = mapper;
        }

        /// <summary>
        /// Serve the page to list flights
        /// </summary>
        /// <param name="airlineId"></param>
        /// <param name="number"></param>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(int airlineId = 0, string number = "", string embarkation = "", string destination = "")
        {
            // Construct the model and assign the selected manufacturer and aircraft
            // model
            ListFlightsViewModel model = new ListFlightsViewModel();
            model.AirlineId = airlineId;
            model.FlightNumber = number;
            model.Embarkation = embarkation;
            model.Destination = destination;

            // Load the airlines list
            List<Airline> airlines = await _airlines.GetAirlinesAsync();
            model.SetAirlines(airlines);

            return View(model);
        }

        /// <summary>
        /// Return a list of flights for the specified airline
        /// </summary>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListByAirline(int airlineId)
        {
            List<Flight> flights = await _flights.GetFlightsByAirlineAsync(airlineId);
            return PartialView("List", flights);
        }

        /// <summary>
        /// Return a list of flights with the specified number
        /// </summary>
        /// <param name="airlineId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListByFlightNumber(string number)
        {
            List<Flight> flights = await _flights.GetFlightsByNumberAsync(number);
            return PartialView("List", flights);
        }

        /// <summary>
        /// Return a list of flights for the specified route
        /// </summary>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListByRoute(string embarkation, string destination)
        {
            List<Flight> flights = await _flights.GetFlightsByRouteAsync(embarkation, destination);
            return PartialView("List", flights);
        }

        /// <summary>
        /// Serve the page to add a new flight
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            AddFlightViewModel model = new AddFlightViewModel();
            List<Airline> airlines = await _airlines.GetAirlinesAsync();
            model.SetAirlines(airlines);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save new flights
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddFlightViewModel model)
        {
            if (ModelState.IsValid)
            {
                string number = model.FlightNumber;
                await _flights.AddFlightAsync(number, model.Embarkation, model.Destination, model.AirlineId);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Flight '{number}' added successfully";
            }

            List<Airline> airlines = await _airlines.GetAirlinesAsync();
            model.SetAirlines(airlines);

            return View(model);
        }

        /// <summary>
        /// Serve the page for editing an existing flight
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Flight flight = await _flights.GetFlightByIdAsync(id);
            EditFlightViewModel model = _mapper.Map<EditFlightViewModel>(flight);
            List<Airline> airlines = await _airlines.GetAirlinesAsync();
            model.SetAirlines(airlines);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated flights
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditFlightViewModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _flights.UpdateFlightAsync(model.Id, model.FlightNumber, model.Embarkation, model.Destination, model.AirlineId);
                result = RedirectToAction("Index", new
                {
                    airlineId = 0,
                    number = model.FlightNumber,
                    embarkation = "",
                    destination = ""
                });
            }
            else
            {
                List<Airline> airlines = await _airlines.GetAirlinesAsync();
                model.SetAirlines(airlines);
                result = View(model);
            }

            return result;
        }
    }
}
