using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class SightingDetailsViewModel : FlightRecorderEntityBase
    {
        public long? SightingId { get; set; }

        [DisplayName("Date")]
        [Required(ErrorMessage = "You must provide a date for the sighting")]
        public DateTime? Date { get; set; }

        [DisplayName("Altitude")]
        [Required(ErrorMessage = "You must provide an altitude")]
        public long? Altitude { get; set; }

        [DisplayName("Location")]
        public long LocationId { get; set; }

        [DisplayName("New Location")]
        public string NewLocation { get; set; }

        [DisplayName("Flight Number")]
        [Required(ErrorMessage = "You must provide a flight number")]
        public string FlightNumber { get; set; }

        [DisplayName("Aircraft Registration")]
        [Required(ErrorMessage = "You must provide an aircraft registration")]
        public string Registration { get; set; }

        [DisplayName("My Flight")]
        public bool IsMyFlight { get; set; }

        public string LastSightingAddedMessage { get; set; }
        public string LocationErrorMessage { get; set; }
        public string Action { get; set; }
        public List<SelectListItem> Locations { get; set; }

        /// <summary>
        /// Set the options for the locations drop-down list
        /// </summary>
        /// <param name="locations"></param>
        public void SetLocations(List<Location> locations)
        {
            // Add the default selection, which is empty
            Locations = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "0", Text = "" }
            };

            // Add the drones retrieved from the service
            if (locations != null)
            {
                Locations.AddRange(locations.Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = $"{x.Name}"
                                    }));
            }
        }
    }
}
