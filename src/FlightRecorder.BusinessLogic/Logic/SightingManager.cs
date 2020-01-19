using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Data;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class SightingManager : ManagerBase<Sighting>, ISightingManager
    {
        private FlightRecorderFactory _factory;

        internal SightingManager(FlightRecorderDbContext context, FlightRecorderFactory factory) : base(context)
        {
            _factory = factory;
        }

        /// <summary>
        /// Add a new sighting
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="date"></param>
        /// <param name="locationId"></param>
        /// <param name="flightId"></param>
        /// <param name="aircraftId"></param>
        /// <returns></returns>
        public Sighting Add(long altitude, DateTime date, long locationId, long flightId, long aircraftId)
        {
            Sighting sighting = Add(new Sighting
            {
                Altitude = altitude,
                Date = date,
                LocationId = locationId,
                FlightId = flightId,
                AircraftId = aircraftId
            });

            return sighting;
        }

        /// <summary>
        /// Add a sighting to the database based on a flattened representation of a sighting
        /// </summary>
        /// <param name="flattened"></param>
        /// <returns></returns>
        public Sighting Add(FlattenedSighting flattened)
        {
            long yearOfManufacture = DateTime.Now.Year - flattened.Age;
            long aircraftId = _factory.Aircraft.Add(flattened.Registration, flattened.SerialNumber, yearOfManufacture, flattened.Model, flattened.Manufacturer).Id;
            long flightId = _factory.Flights.Add(flattened.FlightNumber, flattened.Embarkation, flattened.Destination, flattened.Airline).Id;
            long locationId = _factory.Locations.Add(flattened.Location).Id;
            return Add(flattened.Altitude, flattened.Date, locationId, flightId, aircraftId);
        }

        /// <summary>
        /// Get the first sighting matching the specified criteria along with the associated entites
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override Sighting Get(Expression<Func<Sighting, bool>> predicate = null)
        {
            Sighting sighting = base.Get(predicate);

            if (sighting != null)
            {
                _context.Entry(sighting).Reference(m => m.Aircraft).Load();
                _context.Entry(sighting.Aircraft).Reference(m => m.Model).Load();
                _context.Entry(sighting.Aircraft.Model).Reference(m => m.Manufacturer).Load();
                _context.Entry(sighting).Reference(m => m.Location).Load();
                _context.Entry(sighting).Reference(m => m.Flight).Load();
                _context.Entry(sighting.Flight).Reference(m => m.Airline).Load();
            }

            return sighting;
        }

        /// <summary>
        /// Get the sightings matching the specified criteria along with the associated entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public override IEnumerable<Sighting> List(Expression<Func<Sighting, bool>> predicate = null)
        {
            IEnumerable<Sighting> sightings = base.List(predicate);

            foreach (Sighting sighting in sightings)
            {
                _context.Entry(sighting).Reference(m => m.Aircraft).Load();
                _context.Entry(sighting.Aircraft).Reference(m => m.Model).Load();
                _context.Entry(sighting.Aircraft.Model).Reference(m => m.Manufacturer).Load();
                _context.Entry(sighting).Reference(m => m.Location).Load();
                _context.Entry(sighting).Reference(m => m.Flight).Load();
                _context.Entry(sighting.Flight).Reference(m => m.Airline).Load();
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings of a specified aircraft
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByAircraft(string registration)
        {
            IEnumerable<Sighting> sightings = null;

            Aircraft aircraft = _factory.Aircraft.Get(a => a.Registration == registration);
            if (aircraft != null)
            {
                sightings = List(s => s.AircraftId == aircraft.Id);
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings for a specified route
        /// </summary>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByRoute(string embarkation, string destination)
        {
            IEnumerable<Sighting> sightings = null;

            IEnumerable<Flight> flights = _factory.Flights.List(f => (f.Embarkation == embarkation) && (f.Destination == destination));
            if ((flights != null) && flights.Any())
            {
                IEnumerable<long> flightIds = flights.Select(f => f.Id);
                sightings = List(s => flightIds.Contains(s.FlightId));
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings for a specified airline
        /// </summary>
        /// <param name="airlineName"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByAirline(string airlineName)
        {
            IEnumerable<Sighting> sightings = null;

            IEnumerable<Flight> flights = _factory.Flights.ListByAirline(airlineName);
            if ((flights != null) && flights.Any())
            {
                IEnumerable<long> flightIds = flights.Select(f => f.Id);
                sightings = List(s => flightIds.Contains(s.FlightId));
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings at a specified location
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public IEnumerable<Sighting> ListByLocation(string locationName)
        {
            IEnumerable<Sighting> sightings = null;

            Location location = _factory.Locations.Get(a => a.Name == locationName);
            if (location != null)
            {
                sightings = List(s => s.LocationId == location.Id);
            }

            return sightings;
        }
    }
}
