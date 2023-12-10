using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System;

namespace FlightRecorder.Mvc.Entities
{
    [ExcludeFromCodeCoverage]
    public class JobStatus
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Error { get; set; }
    }
}
