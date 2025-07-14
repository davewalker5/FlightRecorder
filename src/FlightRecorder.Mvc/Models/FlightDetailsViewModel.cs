using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlightRecorder.Entities.Attributes;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class FlightDetailsViewModel
    {
        [DisplayName("Flight Number")]
        public string FlightNumber { get; set; }

        [DisplayName("Flight")]
        public long FlightId { get; set; }

        [IATACode]
        [DisplayName("Embarkation")]
        [Required(ErrorMessage = "You must provide a point of embarkation")]
        public string Embarkation { get; set; }

        [IATACode]
        [DisplayName("Destination")]
        [Required(ErrorMessage = "You must provide a destination airport")]
        public string Destination { get; set; }

        [DisplayName("Airline")]
        public long AirlineId { get; set; }

        [DisplayName("New Airline")]
        public string NewAirline { get; set; }
        public bool IsDuplicate { get; set; }
        public Sighting MostRecentSighting { get; set; }
        public long? SightingId { get; set;}

        public string Action { get; set; }
        public string AirlineErrorMessage { get; set; }
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
                new SelectListItem{ Value = "0", Text = "" }
            };

            // Add the airlines retrieved from the service
            if (flights != null)
            {
                Flights.AddRange(flights.Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = $"{x.Embarkation} - {x.Destination} - {x.Airline.Name}"
                                    }));
            }
        }

        /// <summary>
        /// Set the options for the airlines drop-down list
        /// </summary>
        /// <param name="arlines"></param>
        public void SetAirlines(List<Airline> airlines)
        {
            // Add the default selection, which is empty
            Airlines = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "0", Text = "" }
            };

            // Add the airlines retrieved from the service
            if (airlines != null)
            {
                Airlines.AddRange(airlines.Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = $"{x.Name}"
                                    }));
            }
        }
    }
}
