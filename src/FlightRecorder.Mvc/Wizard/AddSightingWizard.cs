using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.Extensions.Options;

namespace FlightRecorder.Mvc.Wizard
{
    public class AddSightingWizard
    {
        private const string SightingDetailsKeyPrefix = "SightingDetails";
        private const string FlightDetailsKeyPrefix = "FlightDetails";
        private const string AircraftDetailsKeyPrefix = "AircraftDetails";

        private LocationClient _locations;
        private FlightClient _flights;
        private AirlineClient _airlines;
        private ManufacturerClient _manufacturers;
        private ModelClient _models;
        private AircraftClient _aircraft;
        private SightingClient _sightings;
        private ICacheWrapper _cache;
        private IOptions<AppSettings> _settings;

        public AddSightingWizard(LocationClient locations,
                                 FlightClient flights,
                                 AirlineClient airlines,
                                 ManufacturerClient manufacturers,
                                 ModelClient models,
                                 AircraftClient aircraft,
                                 SightingClient sightings,
                                 IOptions<AppSettings> settings,
                                 ICacheWrapper cache)
        {
            _locations = locations;
            _flights = flights;
            _airlines = airlines;
            _manufacturers = manufacturers;
            _models = models;
            _aircraft = aircraft;
            _sightings = sightings;
            _settings = settings;
            _cache = cache;
        }

        /// <summary>
        /// Return the available locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
            => await _locations.GetLocationsAsync();

        /// <summary>
        /// Return flights with the specified number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<List<Flight>> GetFlightsAsync(string number)
            => await _flights.GetFlightsByNumberAsync(number);

        /// <summary>
        /// Return the available airlines
        /// </summary>
        /// <returns></returns>
        public async Task<List<Airline>> GetAirlinesAsync()
            => await _airlines.GetAirlinesAsync();

        /// <summary>
        /// Return the available manufacturers
        /// </summary>
        /// <returns></returns>
        public async Task<List<Manufacturer>> GetManufacturersAsync()
            => await _manufacturers.GetManufacturersAsync();

        /// <summary>
        /// Return the available models for a specified manufacturer
        /// </summary>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync(int manufacturerId)
            => await _models.GetModelsAsync(manufacturerId);

        /// <summary>
        /// Retrieve or construct the sighting details model
        /// </summary>
        /// <returns></returns>
        public async Task<SightingDetailsViewModel> GetSightingDetailsModelAsync()
        {
            // Retrieve the model from the cache
            string key = GetCacheKey(SightingDetailsKeyPrefix);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            if (model == null)
            {
                // Not cached, so create a new one
                model = new SightingDetailsViewModel();
            }

            // Set the available locations
            List<Location> locations = await GetLocationsAsync();
            model.SetLocations(locations);

            return model;
        }

        /// <summary>
        /// Retrieve or constuct the flight details model
        /// </summary>
        /// <returns></returns>
        public async Task<FlightDetailsViewModel> GetFlightDetailsModelAsync()
        {
            // Retrieve the model from the cache
            string key = GetCacheKey(FlightDetailsKeyPrefix);
            FlightDetailsViewModel model = _cache.Get<FlightDetailsViewModel>(key);
            if (model == null)
            {
                // Not cached, so create a new one, using the cached sighting details
                // model to supply the flight number
                key = GetCacheKey(SightingDetailsKeyPrefix);
                SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);
                model = new FlightDetailsViewModel{ FlightNumber = sighting.FlightNumber };
            }

            // Set the available airlines
            List<Airline> airlines = await GetAirlinesAsync();
            model.SetAirlines(airlines);

            // Set the matching flight numbers
            List<Flight> flights = await GetFlightsAsync(model.FlightNumber);
            model.SetFlights(flights);

            // If we have any flights, pick the first one as the default selection from
            // which to populate the model
            Flight flight = flights?.FirstOrDefault();
            if (flight != null)
            {
                model.FlightId = flight.Id;
                model.Embarkation = flight.Embarkation;
                model.Destination = flight.Destination;
                model.AirlineId = flight.AirlineId;
            }

            return model;
        }

        /// <summary>
        /// Retrieve or constuct the flight details model
        /// </summary>
        /// <returns></returns>
        public async Task<AircraftDetailsViewModel> GetAircraftDetailsModelAsync()
        {
            // Retrieve the model from the cache
            string key = GetCacheKey(AircraftDetailsKeyPrefix);
            AircraftDetailsViewModel model = _cache.Get<AircraftDetailsViewModel>(key);
            if (model == null)
            {
                // Not cached, so create a new one, using the cached sighting details
                // model to supply the aircraft registration
                key = GetCacheKey(SightingDetailsKeyPrefix);
                SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);
                model = new AircraftDetailsViewModel { Registration = sighting.Registration };
            }

            // Set the list of available manufacturers
            List<Manufacturer> manufacturers = await GetManufacturersAsync();
            model.SetManufacturers(manufacturers);

            // See if this is an existing aircraft
            Aircraft aircraft = await _aircraft.GetAircraftByRegistrationAsync(model.Registration);
            if  (aircraft != null)
            {
                // It it, so assign the aircraft properties
                model.AircraftId = aircraft.Id;
                model.SerialNumber = aircraft.SerialNumber;
                model.ManufacturerId = aircraft.Model.ManufacturerId;
                model.ModelId = aircraft.ModelId;
                model.Age = DateTime.Now.Year - aircraft.Manufactured;

                // Load the models for the aircraft's manufacturer
                List<Model> models = await GetModelsAsync(model.ManufacturerId);
                model.SetModels(models);
            }

            return model;
        }

        /// <summary>
        /// Cache the sighting details view model
        /// </summary>
        /// <param name="model"></param>
        public void CacheSightingDetailsModel(SightingDetailsViewModel model)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix);
            _cache.Set<SightingDetailsViewModel>(key, model, _settings.Value.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the flight details view model
        /// </summary>
        /// <param name="model"></param>
        public void CacheFlightDetailsModel(FlightDetailsViewModel model)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix);
            _cache.Set<FlightDetailsViewModel>(key, model, _settings.Value.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the aircraft details view model
        /// </summary>
        /// <param name="model"></param>
        public void CacheAircraftDetailsModel(AircraftDetailsViewModel model)
        {
            string key = GetCacheKey(AircraftDetailsKeyPrefix);
            _cache.Set<AircraftDetailsViewModel>(key, model, _settings.Value.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Clear the cached sighting details model
        /// </summary>
        public void ClearCachedSightingDetailsModel()
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached flight details model
        /// </summary>
        public void ClearCachedFlightDetailsModel()
        {
            string key = GetCacheKey(FlightDetailsKeyPrefix);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached flight details model
        /// </summary>
        public void ClearCachedAircraftDetailsModel()
        {
            string key = GetCacheKey(AircraftDetailsKeyPrefix);
            _cache.Remove(key);
        }

        /// <summary>
        /// Create a new sighting
        /// </summary>
        public async Task<Sighting> CreateSighting()
        {
            Sighting sighting = null;

            // Retrieve the sighting details from the cache
            string key = GetCacheKey(SightingDetailsKeyPrefix);
            SightingDetailsViewModel details = _cache.Get<SightingDetailsViewModel>(key);
            if (details != null)
            {
                // Create the aircraft and flight, first
                Aircraft aircraft = await RetrieveOrCreateAircraft();
                Flight flight = await RetrieveOrCreateFlight();

                // Create the location, if required
                if (details.LocationId == 0)
                {
                    Location location = await _locations.AddLocationAsync(details.NewLocation);
                    details.LocationId = location.Id;
                }

                // Create the sighting
                sighting = await _sightings.AddSightingAsync(details.Date, details.Altitude, aircraft.Id, flight.Id, details.LocationId);
            }

            // Clear the cached data
            ClearCachedSightingDetailsModel();
            ClearCachedFlightDetailsModel();
            ClearCachedAircraftDetailsModel();

            return sighting;
        }

        /// <summary>
        /// Either retrieve an existing aircraft or create a new one
        /// </summary>
        /// <returns></returns>
        private async Task<Aircraft> RetrieveOrCreateAircraft()
        {
            Aircraft aircraft = null;

            // Retrieve the aircraft details from the cache
            string key = GetCacheKey(AircraftDetailsKeyPrefix);
            AircraftDetailsViewModel details = _cache.Get<AircraftDetailsViewModel>(key);
            if (details != null)
            {
                // If this is a sighting for an existing aircraft, just return it.
                // Otherwise, we need to create a new aircraft
                if (details.AircraftId > 0)
                {
                    aircraft = await _aircraft.GetAircraftByIdAsync(details.AircraftId);
                }
                else
                {
                    if (details.ManufacturerId == 0)
                    {
                        // With no manufacturer selected, we're creating a new manufacturer and model
                        Manufacturer manufacturer = await _manufacturers.AddManufacturerAsync(details.NewManufacturer);
                        Model model = await _models.AddModelAsync(details.NewModel, manufacturer.Id);
                        details.ModelId = model.Id;
                    }
                    else if (details.ModelId == 0)
                    {
                        // With no model selected, we're creating a new model for the selected manufacturer
                        Model model = await _models.AddModelAsync(details.NewModel, details.ManufacturerId);
                        details.ModelId = model.Id;
                    }

                    // Create the aircraft
                    int manufactured = DateTime.Now.Year - details.Age;
                    aircraft = await _aircraft.AddAircraftAsync(details.Registration, details.SerialNumber, manufactured, details.ModelId);
                }
            }

            return aircraft;
        }

        /// <summary>
        /// Either retrieve an existing flight or create a new one
        /// </summary>
        /// <returns></returns>
        private async Task<Flight> RetrieveOrCreateFlight()
        {
            Flight flight = null;

            // Retrieve the flight details from the cache
            string key = GetCacheKey(FlightDetailsKeyPrefix);
            FlightDetailsViewModel details = _cache.Get<FlightDetailsViewModel>(key);
            if (details != null)
            {
                // If this is a sighting for an existing flight, just return it.
                // Otherwise, we need to create a new flight
                if (details.FlightId > 0)
                {
                    flight = await _flights.GetFlightByIdAsync(details.FlightId);
                }
                else
                {
                    if (details.AirlineId == 0)
                    {
                        // If there's no airline selected, we're creating a new one
                        Airline airline = await _airlines.AddAirlineAsync(details.NewAirline);
                        details.AirlineId = airline.Id;
                    }

                    // Create the flight
                    flight = await _flights.AddFlightAsync(details.FlightNumber, details.Embarkation, details.Destination, details.AirlineId);
                }
            }

            return flight;
        }

        /// <summary>
        /// Construct a key for caching data
        /// </summary>
        /// <returns></returns>
        private string GetCacheKey(string prefix)
        {
            // TODO : Append session-specific value
            string key = $"{prefix}.";
            return key;
        }
    }
}
