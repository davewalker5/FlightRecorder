using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlightRecorder.BusinessLogic.Extensions;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.DataExchange;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Exceptions;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.EntityFrameworkCore;

namespace FlightRecorder.BusinessLogic.Database
{
    internal class SightingManager : ISightingManager
    {
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
                                            .OrderBy(s => s.Date)
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
                                            .OrderBy(s => s.Date)
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .AsAsyncEnumerable();
            }

            return sightings;
        }

        /// <summary>
        /// Return the number of sightings in the database
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountAsync()
            => await _factory.Context.Sightings.CountAsync();

        /// <summary>
        /// Return the most recent sighting matching the predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Sighting> GetMostRecent(Expression<Func<Sighting, bool>> predicate)
        {
            List<Sighting> sightings = await _factory.Context.Sightings
                                                             .Include(s => s.Location)
                                                             .Include(s => s.Flight)
                                                             .ThenInclude(f => f.Airline)
                                                             .Include(s => s.Aircraft)
                                                             .ThenInclude(a => a.Model)
                                                             .ThenInclude(m => m.Manufacturer)
                                                             .Where(predicate)
                                                             .OrderByDescending(x => x.Date)
                                                             .Take(1)
                                                             .AsAsyncEnumerable()
                                                             .ToListAsync();
            return sightings.FirstOrDefault();
        }

        /// <summary>
        /// Add a new sighting
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="date"></param>
        /// <param name="locationId"></param>
        /// <param name="flightId"></param>
        /// <param name="aircraftId"></param>
        /// <param name="isMyFlight"></param>
        /// <returns></returns>
        public async Task<Sighting> AddAsync(long altitude, DateTime date, long locationId, long flightId, long aircraftId, bool isMyFlight)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding sighting: Altitude = {altitude}, Date = {date}, Location ID = {locationId}, Flight ID = {flightId}, Aircraft ID = {aircraftId}");

            // Check the location, flight and aircraft all exist
            await _factory.Locations.CheckLocationExists(locationId);
            await _factory.Flights.CheckFlightExists(flightId);
            await _factory.Aircraft.CheckAircraftExists(aircraftId);

            // Add the sighting and save changes
            var sighting = new Sighting
            {
                Altitude = altitude,
                Date = date,
                LocationId = locationId,
                FlightId = flightId,
                AircraftId = aircraftId,
                IsMyFlight = isMyFlight
            };

            await _factory.Context.Sightings.AddAsync(sighting);
            await _factory.Context.SaveChangesAsync();

            // Loading the related entities using the following syntax is problematic if the model and/or
            // sighting are NULL on the aircraft related to the sighting:
            //
            // await _factory.Context.Entry(sighting.Aircraft).Reference(a => a.Model).LoadAsync();
            // await _factory.Context.Entry(sighting.Aircraft.Model).Reference(m => m.Sighting).LoadAsync();
            //
            // Instead, reload the sighting as this handles the above case
            sighting = await GetAsync(x => x.Id == sighting.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"Added sighting {sighting}");
            return sighting;
        }

        /// <summary>
        /// Add a sighting to the database based on a flattened representation of a sighting
        /// </summary>
        /// <param name="flattened"></param>
        /// <returns></returns>
        public async Task<Sighting> AddAsync(FlattenedSighting flattened)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding sighting: {flattened}");

            // The aircraft model can only be created if both the model name and sighting are specified. If they
            // are, create the sighting and model
            long? modelId = null;
            if (!string.IsNullOrEmpty(flattened.Model) && !string.IsNullOrEmpty(flattened.Manufacturer))
            {
                long manufacturerId = (await _factory.Manufacturers.AddIfNotExistsAsync(flattened.Manufacturer)).Id;
                modelId = (await _factory.Models.AddIfNotExistsAsync(flattened.Model, manufacturerId)).Id;
            }

            // Get the year of sighting and create the aircraft
            long? yearOfManufacture = !string.IsNullOrEmpty(flattened.Age) ? DateTime.Now.Year - long.Parse(flattened.Age) : null;
            long aircraftId = (await _factory.Aircraft.AddIfNotExistsAsync(flattened.Registration, flattened.SerialNumber, yearOfManufacture, modelId)).Id;

            // Add the airline and flight details
            long airlineId = (await _factory.Airlines.AddIfNotExistsAsync(flattened.Airline)).Id;
            long flightId = (await _factory.Flights.AddIfNotExistsAsync(flattened.FlightNumber, flattened.Embarkation, flattened.Destination, airlineId)).Id;
            long locationId = (await _factory.Locations.AddIfNotExistsAsync(flattened.Location)).Id;

            // Finally, add the sighting
            var sighting = await AddAsync(flattened.Altitude, flattened.Date, locationId, flightId, aircraftId, flattened.IsMyFlight);

            _factory.Logger.LogMessage(Severity.Debug, $"Added sighting {sighting}");

            return sighting;
        }

        /// <summary>
        /// Update a sighting
        /// </summary>
        /// <param name="id"></param>
        /// <param name="altitude"></param>
        /// <param name="date"></param>
        /// <param name="locationId"></param>
        /// <param name="flightId"></param>
        /// <param name="aircraftId"></param>
        /// <param name="isMyFlight"></param>
        /// <returns></returns>
        public async Task<Sighting> UpdateAsync(long id, long altitude, DateTime date, long locationId, long flightId, long aircraftId, bool isMyFlight)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating sighting: ID = {id}, Altitude = {altitude}, Date = {date}, Location ID = {locationId}, Flight ID = {flightId}, Aircraft ID = {aircraftId}");

            // Retrieve the sighting
            var sighting = await _factory.Context.Sightings.FirstOrDefaultAsync(x => x.Id == id);
            if (sighting == null)
            {
                var message = $"Sighting with ID {id} not found";
                throw new SightingNotFoundException(message);
            }

            // Check the location, flight and aircraft all exist
            await _factory.Locations.CheckLocationExists(locationId);
            await _factory.Flights.CheckFlightExists(flightId);
            await _factory.Aircraft.CheckAircraftExists(aircraftId);

            // Update sighting properties and save changes
            sighting.Date = date;
            sighting.AircraftId = aircraftId;
            sighting.Altitude = altitude;
            sighting.FlightId = flightId;
            sighting.LocationId = locationId;
            sighting.IsMyFlight = isMyFlight;
            await _factory.Context.SaveChangesAsync();

            // Loading the related entities using the following syntax is problematic if the model and/or
            // sighting are NULL on the aircraft related to the sighting:
            //
            // await _factory.Context.Entry(sighting.Aircraft).Reference(a => a.Model).LoadAsync();
            // await _factory.Context.Entry(sighting.Aircraft.Model).Reference(m => m.Sighting).LoadAsync();
            //
            // Instead, reload the sighting as this handles the above case
            sighting = await GetAsync(x => x.Id == sighting.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"Updated sighting {sighting}");

            return sighting;
        }

        /// <summary>
        /// Delete the sighting with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="SightingNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting sighting: ID = {id}");

            // Check the sighting exists
            var sighting = await _factory.Context.Sightings.FirstOrDefaultAsync(x => x.Id == id);
            if (sighting == null)
            {
                var message = $"Sighting with ID {id} not found";
                throw new SightingNotFoundException(message);
            }

            // Remove the sighting
            _factory.Context.Remove(sighting);
            await _factory.Context.SaveChangesAsync();
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
                                                                 (f.Destination == destination), 1, int.MaxValue)
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
            IAsyncEnumerable<Flight> matches = await _factory.Flights.ListByAirlineAsync(airlineName, 1, int.MaxValue);
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
