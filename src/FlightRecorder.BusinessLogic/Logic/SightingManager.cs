using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class SightingManager : ISightingManager
    {
        private const int AllFlightsPageSize = 1000000;

        private readonly FlightRecorderFactory _factory;

        internal SightingManager(FlightRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Get the first sighting matching the specified criteria along with the associated entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate)
        {
            List<Sighting> sightings = await ListAsync(predicate, 1, 1).ToListAsync();
            return sightings.FirstOrDefault();
        }

        /// <summary>
        /// Get the sightings matching the specified criteria along with the associated entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Sighting> sightings;

            if (predicate == null)
            {
                sightings = _factory.Context.Sightings
                                            .Include(s => s.Location)
                                            .Include(s => s.Flight)
                                            .ThenInclude(f => f.Airline)
                                            .Include(s => s.Aircraft)
                                            .ThenInclude(a => a.Model)
                                            .ThenInclude(m => m.Manufacturer)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .AsAsyncEnumerable();
            }
            else
            {
                sightings = _factory.Context.Sightings
                                            .Include(s => s.Location)
                                            .Include(s => s.Flight)
                                            .ThenInclude(f => f.Airline)
                                            .Include(s => s.Aircraft)
                                            .ThenInclude(a => a.Model)
                                            .ThenInclude(m => m.Manufacturer)
                                            .Where(predicate)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .AsAsyncEnumerable();
            }

            return sightings;
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
        public async Task<Sighting> AddAsync(long altitude, DateTime date, long locationId, long flightId, long aircraftId)
        {
            Sighting sighting = new Sighting
            {
                Altitude = altitude,
                Date = date,
                LocationId = locationId,
                FlightId = flightId,
                AircraftId = aircraftId
            };

            await _factory.Context.Sightings.AddAsync(sighting);
            await _factory.Context.SaveChangesAsync();

            // Loading the related entities using the following syntax is problematic if the model and/or
            // manufacturer are NULL on the aircraft related to the sighting:
            //
            // await _factory.Context.Entry(sighting.Aircraft).Reference(a => a.Model).LoadAsync();
            // await _factory.Context.Entry(sighting.Aircraft.Model).Reference(m => m.Manufacturer).LoadAsync();
            //
            // Instead, reload the sighting as this handles the above case
            sighting = await GetAsync(x => x.Id == sighting.Id);
            return sighting;
        }

        /// <summary>
        /// Add a sighting to the database based on a flattened representation of a sighting
        /// </summary>
        /// <param name="flattened"></param>
        /// <returns></returns>
        public async Task<Sighting> AddAsync(FlattenedSighting flattened)
        {
            long? yearOfManufacture = !string.IsNullOrEmpty(flattened.Age) ? DateTime.Now.Year - long.Parse(flattened.Age) : null;
            long aircraftId = (await _factory.Aircraft.AddAsync(flattened.Registration, flattened.SerialNumber, yearOfManufacture, flattened.Model, flattened.Manufacturer)).Id;
            long flightId = (await _factory.Flights.AddAsync(flattened.FlightNumber, flattened.Embarkation, flattened.Destination, flattened.Airline)).Id;
            long locationId = (await _factory.Locations.AddAsync(flattened.Location)).Id;
            return await AddAsync(flattened.Altitude, flattened.Date, locationId, flightId, aircraftId);
        }

        /// <summary>
        /// Return a list of sightings of a specified aircraft
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Sighting>> ListByAircraftAsync(string registration, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Sighting> sightings = null;

            registration = registration.CleanString().ToUpper();
            Aircraft aircraft = await _factory.Aircraft.GetAsync(a => a.Registration == registration);
            if (aircraft != null)
            {
                sightings = ListAsync(s => s.AircraftId == aircraft.Id, pageNumber, pageSize);
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings for a specified route
        /// </summary>
        /// <param name="embarkation"></param>
        /// <param name="destination"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Sighting>> ListByRouteAsync(string embarkation, string destination, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Sighting> sightings = null;

            embarkation = embarkation.CleanString().ToUpper();
            destination = destination.CleanString().ToUpper();
            List<Flight> flights = await _factory.Flights
                                                 .ListAsync(f => (f.Embarkation == embarkation) &&
                                                                 (f.Destination == destination), 1, AllFlightsPageSize)
                                                 .ToListAsync();
            if (flights.Any())
            {
                IEnumerable<long> flightIds = flights.Select(f => f.Id);
                sightings = ListAsync(s => flightIds.Contains(s.FlightId), pageNumber, pageSize);
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings for a specified airline
        /// </summary>
        /// <param name="airlineName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Sighting>> ListByAirlineAsync(string airlineName, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Sighting> sightings = null;

            airlineName = airlineName.CleanString();
            IAsyncEnumerable<Flight> matches = await _factory.Flights.ListByAirlineAsync(airlineName, 1, AllFlightsPageSize);
            if (matches != null)
            {
                List<Flight> flights = await matches.ToListAsync();
                if (flights.Any())
                {
                    IEnumerable<long> flightIds = flights.Select(f => f.Id);
                    sightings = ListAsync(s => flightIds.Contains(s.FlightId), pageNumber, pageSize);
                }
            }

            return sightings;
        }

        /// <summary>
        /// Return a list of sightings at a specified location
        /// </summary>
        /// <param name="locationName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IAsyncEnumerable<Sighting>> ListByLocationAsync(string locationName, int pageNumber, int pageSize)
        {
            IAsyncEnumerable<Sighting> sightings = null;

            locationName = locationName.CleanString();
            Location location = await _factory.Locations.GetAsync(a => a.Name == locationName);
            if (location != null)
            {
                sightings = ListAsync(s => s.LocationId == location.Id, pageNumber, pageSize);
            }

            return sightings;
        }
    }
}
