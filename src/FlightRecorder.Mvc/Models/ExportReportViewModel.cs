using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class ExportReportViewModel : ExportViewModel
    {
        public readonly List<string> ReportTypeNames = new()
        {
            "Airline Statistics",
            "Location Statistics",
            "Manufacturer Statistics",
            "Model Statistics",
            "Flights By Month"
        };

        [DisplayName("Report Type")]
        [Required(ErrorMessage = "You must select a report type")]
        public int? ReportType { get; set; }

        public List<SelectListItem> ReportTypes { get; private set; }

        public ExportReportViewModel()
        {
            // Initialise the report types selection, starting with an empty item (no selection)
            ReportTypes = new List<SelectListItem>()
            {
                new SelectListItem("", "")
            };

            // Add each of the report types
            for (int i = 0; i < ReportTypeNames.Count; i++)
            {
                ReportTypes.Add(new SelectListItem(ReportTypeNames[i], i.ToString()));
            }
        }

        /// <summary>
        /// Clear all the model's fields
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            ReportType = null;
        }
    }
}
