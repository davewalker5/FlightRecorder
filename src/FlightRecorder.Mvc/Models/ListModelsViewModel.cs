using System.Collections.Generic;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public class ListModelsViewModel : ModelViewModelBase
    {
        public List<Model> Models { get; set; }
    }
}
