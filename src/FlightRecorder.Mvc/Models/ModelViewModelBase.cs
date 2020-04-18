using System.Collections.Generic;
using System.Linq;
using FlightRecorder.Mvc.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public abstract class ModelViewModelBase : Model
    {
        public List<SelectListItem> Manufacturers { get; private set; }

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
    }
}
