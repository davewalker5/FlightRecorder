using System;
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
            CreateMap<Model, AddModelViewModel>();
            CreateMap<Model, EditModelViewModel>();
            CreateMap<Aircraft, AddAircraftViewModel>()
                .ForMember(m => m.Age, x => x.MapFrom(m => DateTime.Now.Year - m.Manufactured))
                .ForMember(m => m.Id, x => x.MapFrom(m => m.Id))
                .ForMember(m => m.ManufacturerId, x => x.MapFrom(m => m.Model.ManufacturerId))
                .ForMember(m => m.Manufacturers, x => x.Ignore())
                .ForMember(m => m.ModelId, x => x.MapFrom(m => m.ModelId))
                .ForMember(m => m.Models, x => x.Ignore())
                .ForMember(m => m.Registration, x => x.MapFrom(m => m.Registration))
                .ForMember(m => m.SerialNumber, x => x.MapFrom(m => m.SerialNumber));
            CreateMap<Aircraft, EditAircraftViewModel>()
                .ForMember(m => m.Age, x => x.MapFrom(m => DateTime.Now.Year - m.Manufactured))
                .ForMember(m => m.Id, x => x.MapFrom(m => m.Id))
                .ForMember(m => m.ManufacturerId, x => x.MapFrom(m => m.Model.ManufacturerId))
                .ForMember(m => m.Manufacturers, x => x.Ignore())
                .ForMember(m => m.ModelId, x => x.MapFrom(m => m.ModelId))
                .ForMember(m => m.Models, x => x.Ignore())
                .ForMember(m => m.Registration, x => x.MapFrom(m => m.Registration))
                .ForMember(m => m.SerialNumber, x => x.MapFrom(m => m.SerialNumber));
        }
    }
}
