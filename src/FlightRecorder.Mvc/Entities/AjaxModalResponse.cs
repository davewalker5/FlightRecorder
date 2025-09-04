using FlightRecorder.Entities.Db;

namespace HealthTracker.Mvc.Models
{
    public class AjaxModalResponse : FlightRecorderEntityBase
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }
    }
}
