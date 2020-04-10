using System;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.Data;
using FlightRecorder.Entities.Interfaces;

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

        public IAirlineManager Airlines { get { return _airlines.Value; } }
        public ILocationManager Locations { get { return _locations.Value; } }
        public IManufacturerManager Manufacturers { get { return _manufacturers.Value; } }
        public IModelManager Models { get { return _models.Value; } }
        public IAircraftManager Aircraft { get { return _aircraft.Value; } }
        public IFlightManager Flights { get { return _flights.Value; } }
        public ISightingManager Sightings { get { return _sightings.Value; } }
        public FlightRecorderDbContext Context { get; private set; }

        public FlightRecorderFactory(FlightRecorderDbContext context)
        {
            Context = context;
            _airlines = new Lazy<IAirlineManager>(() => new AirlineManager(context));
            _locations = new Lazy<ILocationManager>(() => new LocationManager(context));
            _manufacturers = new Lazy<IManufacturerManager>(() => new ManufacturerManager(context));
            _models = new Lazy<IModelManager>(() => new ModelManager(this));
            _aircraft = new Lazy<IAircraftManager>(() => new AircraftManager(this));
            _flights = new Lazy<IFlightManager>(() => new FlightManager(context, this));
            _sightings = new Lazy<ISightingManager>(() => new SightingManager(context, this));
        }
    }
}
