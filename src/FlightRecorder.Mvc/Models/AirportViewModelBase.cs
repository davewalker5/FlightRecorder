using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlightRecorder.Entities.Db;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightRecorder.Mvc.Models
{
    public class AirportViewModelBase
    {
        public int Id { get; set; }

        [DisplayName("Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must provide a country")]
        public int CountryId { get; set; }

        [DisplayName("IATA Code")]
        [Required(ErrorMessage = "You must provide an IATA code")]
        public string Code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        public List<SelectListItem> Countries { get; set; }

        /// <summary>
        /// Set the options for the countries drop-down list
        /// </summary>
        /// <param name="countries"></param>
        public void SetCountries(List<Country> countries)
        {
            // Add the default selection, which is empty
            Countries = new List<SelectListItem>()
            {
                new SelectListItem{ Value = "", Text = "" }
            };

            // Add the drones retrieved from the service
            if (countries != null)
            {
                Countries.AddRange(countries.Select(a =>
                                    new SelectListItem
                                    {
                                        Value = a.Id.ToString(),
                                        Text = $"{a.Name}"
                                    }));
            }
        }
    }
}