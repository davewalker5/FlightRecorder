using System.Collections.Generic;
using FlightRecorder.Mvc.Entities;

namespace FlightRecorder.Mvc.Models
{
    public class ListModelsViewModel : ModelViewModelBase
    {
        public List<Model> Models { get; set; }
    }
}
