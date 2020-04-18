using AutoMapper;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Models;

namespace FlightRecorder.Mvc.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Manufacturer, AddManufacturerViewModel>();
            CreateMap<Location, AddLocationViewModel>();
            // CreateMap<Model, AddModelViewModel>();
            CreateMap<Model, EditModelViewModel>();
        }
    }
}
