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

            CreateMap<Flight, EditFlightViewModel>()
                .ForMember(m => m.AirlineId, x => x.MapFrom(m => m.AirlineId))
                .ForMember(m => m.Airlines, x => x.Ignore())
                .ForMember(m => m.Destination, x => x.MapFrom(m => m.Destination))
                .ForMember(m => m.Embarkation, x => x.MapFrom(m => m.Embarkation))
                .ForMember(m => m.FlightNumber, x => x.MapFrom(m => m.Number))
                .ForMember(m => m.Id, x => x.MapFrom(m => m.Id));

            CreateMap<SightingDetailsViewModel, ConfirmDetailsViewModel>()
                .ForMember(m => m.Altitude, x => x.MapFrom(m => m.Altitude))
                .ForMember(m => m.Date, x => x.MapFrom(m => m.Date));

            CreateMap<SightingDetailsViewModel, ConfirmDetailsViewModel>()
                .ForMember(m => m.Altitude, x => x.MapFrom(m => m.Altitude))
                .ForMember(m => m.Date, x => x.MapFrom(m => m.Date));

            CreateMap<FlightDetailsViewModel, ConfirmDetailsViewModel>()
                .ForMember(m => m.FlightNumber, x => x.MapFrom(m => m.FlightNumber))
                .ForMember(m => m.Embarkation, x => x.MapFrom(m => m.Embarkation))
                .ForMember(m => m.Destination, x => x.MapFrom(m => m.Destination));

            CreateMap<AircraftDetailsViewModel, ConfirmDetailsViewModel>()
                .ForMember(m => m.Registration, x => x.MapFrom(m => m.Registration))
                .ForMember(m => m.SerialNumber, x => x.MapFrom(m => m.SerialNumber))
                .ForMember(m => m.Age, x => x.MapFrom(m => m.Age));

            CreateMap<Airport, EditAirportViewModel>()
                .ForMember(m => m.CountryId, x => x.MapFrom(m => m.CountryId))
                .ForMember(m => m.Countries, x => x.Ignore())
                .ForMember(m => m.Code, x => x.MapFrom(m => m.Code))
                .ForMember(m => m.Name, x => x.MapFrom(m => m.Name))
                .ForMember(m => m.Id, x => x.MapFrom(m => m.Id));
        }
    }
}
