using FlightRecorder.Entities.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FlightRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class JobStatus
    {
        [Key]
        public long Id { get; set; }

        [Export("Job Name", 1)]
        public string Name { get; set; }

        [Export("Parameters", 2)]
        public string Parameters { get; set; }

        [Export("Started", 3)]
        public DateTime Start { get; set; }

        [Export("Completed", 4)]
        public DateTime? End { get; set; }

        [Export("Errors", 5)]
        public string Error { get; set; }
    }
}
