using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class AircraftDetailsViewModel
    {
        public long? AircraftId { get; set; }

        [DisplayName("Aircraft Registration")]
        [Required(ErrorMessage = "You must provide an aircraft registration")]
        public string Registration { get; set; }

        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }

        [DisplayName("Age")]
        [Range(0, int.MaxValue, ErrorMessage = "Aircraft age is not valid")]
        public long? Age { get; set; }

        [DisplayName("Manufacturer")]
        public long? ManufacturerId { get; set; }

        [DisplayName("New Manufacturer")]
        public string NewManufacturer { get; set; }

        [DisplayName("Model")]
        public long? ModelId { get; set; }
        public long? DropDownModelId { get; set; }

        [DisplayName("New Model")]
        public string NewModel { get; set; }
        public Sighting MostRecentSighting { get; set; }

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
                new SelectListItem{ Value = "0", Text = "" }
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
                new SelectListItem{ Value = "0", Text = "" }
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
