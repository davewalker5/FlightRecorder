using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public class ExportViewModel
    {
        [DisplayName("File Name")]
        [Required(ErrorMessage = "You must specify an export file name")]
        public string FileName { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Clear all the model's fields
        /// </summary>
        public virtual void Clear()
        {
            FileName = null;
        }
    }
}
