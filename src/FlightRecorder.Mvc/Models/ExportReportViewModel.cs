using FlightRecorder.Entities.Reporting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class ExportReportViewModel : ExportViewModel
    {
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
            foreach (var definition in ReportDefinitions.Definitions)
            {
                ReportTypes.Add(new SelectListItem(definition.DisplayName, ((int)definition.ReportType).ToString()));
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
