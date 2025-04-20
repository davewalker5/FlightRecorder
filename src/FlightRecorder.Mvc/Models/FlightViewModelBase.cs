using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlightRecorder.Mvc.Attributes;
using FlightRecorder.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public abstract class FlightViewModelBase
    {
        public int Id { get; set; }

        [DisplayName("Airline")]
        [Range(1, int.MaxValue, ErrorMessage = "You must provide an airline")]
        public int AirlineId { get; set; }

        [DisplayName("Flight Number")]
        [Required(ErrorMessage = "You must provide a flight number")]
        public string FlightNumber { get; set; }

        [IATACode]
        [DisplayName("Point of Embarkation")]
        [Required(ErrorMessage = "You must provide a point of embarkation")]
        public string Embarkation { get; set; }

        [IATACode]
        [DisplayName("Destination Airport")]
        [Required(ErrorMessage = "You must provide a destination airport")]
        public string Destination { get; set; }

        public List<SelectListItem> Airlines { get; set; }

        /// <summary>
        /// Set the options for the airlines drop-down list
        /// </summary>
        /// <param name="airlines"></param>
        public void SetAirlines(List<Airline> airlines)
        {
            // Add the default selection, which is empty
            Airlines = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the airlines retrieved from the service
            if (airlines != null)
            {
                Airlines.AddRange(airlines.Select(a =>
                                    new SelectListItem
                                    {
                                        Value = a.Id.ToString(),
                                        Text = $"{a.Name}"
                                    }));
            }
        }
    }
}
