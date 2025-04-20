using System.Collections.Generic;
using System.ComponentModel;

namespace FlightRecorder.Mvc.Models
{
    public class CacheManagementViewModel
    {
        public string Message { get; set; }


        [DisplayName("Filter")]
        public string Filter { get; set; }

        public void Clear()
        {
            Message = "";
            Filter = "";
        }
    }
}
