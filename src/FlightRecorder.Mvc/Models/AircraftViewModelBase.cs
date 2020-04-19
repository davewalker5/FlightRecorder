using System.Collections.Generic;
using System.Linq;
using FlightRecorder.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public abstract class AircraftViewModelBase : Aircraft
    {
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
            Manufacturers.AddRange(manufacturers.Select(m =>
                                    new SelectListItem
                                    {
                                        Value = m.Id.ToString(),
                                        Text = $"{m.Name}"
                                    }));
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
            Models.AddRange(models.Select(m =>
                            new SelectListItem
                            {
                                Value = m.Id.ToString(),
                                Text = $"{m.Name}"
                            }));
        }
    }
}
