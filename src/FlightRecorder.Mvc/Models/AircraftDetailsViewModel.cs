using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlightRecorder.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class AircraftDetailsViewModel
    {
        public int AircraftId { get; set; }

        [DisplayName("Aircraft Registration")]
        [Required(ErrorMessage = "You must provide an aircraft registration")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        [Required(ErrorMessage = "You must provide a serial number")]
        public string SerialNumber { get; set; }

        [DisplayName("Age")]
        [Range(0, int.MaxValue, ErrorMessage = "You must provide an aircraft age")]
        public int Age { get; set; }

        [DisplayName("Manufacturer")]
        public int ManufacturerId { get; set; }

        [DisplayName("New Manufacturer")]
        public string NewManufacturer { get; set; }

        [DisplayName("Model")]
        public int ModelId { get; set; }

        [DisplayName("New Model")]
        public string NewModel { get; set; }

        public string Action { get; set; }

        public List<SelectListItem> Manufacturers { get; set; }
        public List<SelectListItem> Models { get; set; }

        /// <summary>
        /// Set the options for the manufacturers drop-down list
        /// </summary>
        /// <param name="manufacturers"></param>
        public void SetManufacturers(List<Manufacturer> manufacturers)
        {
            // Add the default selection, which is empty
            Manufacturers = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the drones retrieved from the service
            if (manufacturers != null)
            {
                Manufacturers.AddRange(manufacturers.Select(m =>
                                        new SelectListItem
                                        {
                                            Value = m.Id.ToString(),
                                            Text = $"{m.Name}"
                                        }));
            }
        }

        /// <summary>
        /// Set the options for the models drop-down list
        /// </summary>
        /// <param name="models"></param>
        public void SetModels(List<Model> models)
        {
            // Add the default selection, which is empty
            Models = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the drones retrieved from the service
            if (models != null)
            {
                Models.AddRange(models.Select(m =>
                                new SelectListItem
                                {
                                    Value = m.Id.ToString(),
                                    Text = $"{m.Name}"
                                }));
            }
        }
    }
}
