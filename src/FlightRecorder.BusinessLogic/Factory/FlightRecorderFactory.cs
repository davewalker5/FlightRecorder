using System;
using System.Diagnostics.CodeAnalysis;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.Data;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Reporting;

namespace FlightRecorder.BusinessLogic.Factory
{
    public class FlightRecorderFactory
    {
        private readonly Lazy<IAirlineManager> _airlines = null;
        private readonly Lazy<ILocationManager> _locations = null;
        private readonly Lazy<IManufacturerManager> _manufacturers = null;
        private readonly Lazy<IModelManager> _models = null;
        private readonly Lazy<IAircraftManager> _aircraft = null;
        private readonly Lazy<IFlightManager> _flights = null;
        private readonly Lazy<ISightingManager> _sightings = null;
        private readonly Lazy<IUserManager> _users = null;
        private readonly Lazy<ICountryManager> _countries = null;
        private readonly Lazy<IAirportManager> _airports = null;
        private readonly Lazy<IDateBasedReport<AirlineStatistics>> _airlineStatistics = null;
        private readonly Lazy<IDateBasedReport<LocationStatistics>> _locationStatistics = null;
        private readonly Lazy<IDateBasedReport<ManufacturerStatistics>> _manufacturerStatistics = null;
        private readonly Lazy<IDateBasedReport<ModelStatistics>> _modelStatistics = null;
        public FlightRecorderDbContext Context { get; private set; }
        public IAirlineManager Airlines { get { return _airlines.Value; } }
        public ILocationManager Locations { get { return _locations.Value; } }
        public IManufacturerManager Manufacturers { get { return _manufacturers.Value; } }
        public IModelManager Models { get { return _models.Value; } }
        public IAircraftManager Aircraft { get { return _aircraft.Value; } }
        public IFlightManager Flights { get { return _flights.Value; } }
        public ISightingManager Sightings { get { return _sightings.Value; } }
        public IUserManager Users { get { return _users.Value; } }
        public ICountryManager Countries { get { return _countries.Value; } }
        public IAirportManager Airports { get { return _airports.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<AirlineStatistics> AirlineStatistics { get { return _airlineStatistics.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<LocationStatistics> LocationStatistics { get { return _locationStatistics.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<ManufacturerStatistics> ManufacturerStatistics { get { return _manufacturerStatistics.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<ModelStatistics> ModelStatistics { get { return _modelStatistics.Value; } }

        public FlightRecorderFactory(FlightRecorderDbContext context)
        {
            Context = context;
            _airlines = new Lazy<IAirlineManager>(() => new AirlineManager(context));
            _locations = new Lazy<ILocationManager>(() => new LocationManager(context));
            _manufacturers = new Lazy<IManufacturerManager>(() => new ManufacturerManager(context));
            _models = new Lazy<IModelManager>(() => new ModelManager(this));
            _aircraft = new Lazy<IAircraftManager>(() => new AircraftManager(this));
            _flights = new Lazy<IFlightManager>(() => new FlightManager(this));
            _sightings = new Lazy<ISightingManager>(() => new SightingManager(this));
            _users = new Lazy<IUserManager>(() => new UserManager(context));
            _countries = new Lazy<ICountryManager>(() => new CountryManager(context));
            _airports = new Lazy<IAirportManager>(() => new AirportManager(this));
            _airlineStatistics = new Lazy<IDateBasedReport<AirlineStatistics>>(() => new DateBasedReport<AirlineStatistics>(context));
            _locationStatistics = new Lazy<IDateBasedReport<LocationStatistics>>(() => new DateBasedReport<LocationStatistics>(context));
            _manufacturerStatistics = new Lazy<IDateBasedReport<ManufacturerStatistics>>(() => new DateBasedReport<ManufacturerStatistics>(context));
            _modelStatistics = new Lazy<IDateBasedReport<ModelStatistics>>(() => new DateBasedReport<ModelStatistics>(context));
        }
    }
}
