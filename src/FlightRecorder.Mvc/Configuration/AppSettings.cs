using System.Collections.Generic;

namespace FlightRecorder.Mvc.Configuration
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ApiUrl { get; set; }
        public List<ApiRoute> ApiRoutes { get; set; }
        public string ApiDateFormat { get; set; }
        public int SearchPageSize { get; set; }
        public int PropertiesPerRow { get; set; }
        public int CacheLifetimeSeconds { get; set; }
        public string DateTimeFormat { get; set; }
        public string DefaultLocationAttribute { get; set; }
    }
}
