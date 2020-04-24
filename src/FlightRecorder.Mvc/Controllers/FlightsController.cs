using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightRecorder.Mvc.Controllers
{
    [Authorize]
    public class FlightsController : Controller
    {
        private readonly AirlineClient _airlines;
        private readonly FlightClient _flights;
        private readonly IMapper _mapper;

        public FlightsController(AirlineClient airlines, FlightClient flights, IMapper mapper)
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
            List<Flight> flights = await _flights.GetFlightsByAirline(airlineId);
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
            List<Flight> flights = await _flights.GetFlightsByNumber(number);
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
            List<Flight> flights = await _flights.GetFlightsByRoute(embarkation, destination);
            return PartialView("List", flights);
        }
    }
}
