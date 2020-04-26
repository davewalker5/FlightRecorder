using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlightRecorder.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class FlightDetailsViewModel
    {
        [DisplayName("Flight Number")]
        [Required(ErrorMessage = "You must provide a flight number")]
        public string FlightNumber { get; set; }

        [DisplayName("Flight Id")]
        public int FlightId { get; set; }

        [DisplayName("Registration")]
        public string Embarkation { get; set; }

        [DisplayName("Registration")]
        public string Destination { get; set; }

        [DisplayName("Airline")]
        public int AirlineId { get; set; }

        [DisplayName("New Airline")]
        public string NewAirline { get; set; }

        public string Action { get; set; }

        public List<SelectListItem> Flights { get; set; }
        public List<SelectListItem> Airlines { get; set; }

        /// <summary>
        /// Set the options for the airlines drop-down list
        /// </summary>
        /// <param name="flights"></param>
        public void SetFlights(List<Flight> flights)
        {
            // Add the default selection, which is empty
            Flights = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the drones retrieved from the service
            Flights.AddRange(flights.Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Id.ToString(),
                                    Text = $"{x.Embarkation} - {x.Destination} - {x.Airline.Name}"
                                }));
        }

        /// <summary>
        /// Set the options for the airlines drop-down list
        /// </summary>
        /// <param name="arlines"></param>
        public void SetAirlines(List<Airline> arlines)
        {
            // Add the default selection, which is empty
            Airlines = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the drones retrieved from the service
            Airlines.AddRange(arlines.Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Id.ToString(),
                                    Text = $"{x.Name}"
                                }));
        }
    }
}
