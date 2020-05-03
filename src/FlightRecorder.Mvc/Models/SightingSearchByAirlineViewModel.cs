using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlightRecorder.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class SightingSearchByAirlineViewModel : SightingsSearchViewModelBase
    {
        [DisplayName("Airline")]
        [Required(ErrorMessage = "You must provide an airline")]
        public int AirlineId { get; set; }

        public List<SelectListItem> Airlines { get; set; }

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
            if (Airlines != null)
            {
                Airlines.AddRange(arlines.Select(a =>
                                    new SelectListItem
                                    {
                                        Value = a.Id.ToString(),
                                        Text = $"{a.Name}"
                                    }));
            }
        }
    }
}
