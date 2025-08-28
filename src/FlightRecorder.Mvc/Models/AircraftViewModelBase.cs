using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public abstract class AircraftViewModelBase
    {
        public int Id { get; set; }

        [DisplayName("Manufacturer")]
        [Range(1, int.MaxValue, ErrorMessage = "You must provide a manufacturer")]
        public int ManufacturerId { get; set; }

        [DisplayName("Model")]
        [Range(1, int.MaxValue, ErrorMessage = "You must provide a model")]
        public int ModelId { get; set; }

        [DisplayName("Registration")]
        [Required(ErrorMessage = "You must provide a registration number")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Age")]
        [Range(0, int.MaxValue, ErrorMessage = "Aircraft age is not valid")]
        public int? Age { get; set; }

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
