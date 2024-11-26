using AutoMapper;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Entities;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Models;
using Microsoft.Extensions.Options;
using System.Text;

namespace FlightRecorder.Mvc.Wizard
{
    public class AddSightingWizard
    {
        private const string SightingDetailsKeyPrefix = "Wizard.SightingDetails";
        private const string FlightDetailsKeyPrefix = "Wizard.FlightDetails";
        private const string AircraftDetailsKeyPrefix = "Wizard.AircraftDetails";
        private const string LastSightingAddedKeyPrefix = "Wizard.LastSightingAdded";
        private const string DefaultDateKeyPrefix = "Wizard.DefaultDate";
        private const string DefaultLocationKeyPrefix = "Wizard.DefaultLocation";

        private readonly LocationClient _locations;
        private readonly FlightClient _flights;
        private readonly AirlineClient _airlines;
        private readonly ManufacturerClient _manufacturers;
        private readonly ModelClient _models;
        private readonly AircraftClient _aircraft;
        private readonly SightingClient _sightings;
        private readonly SightingsSearchClient _sightingsSearch;
        private readonly UserAttributesClient _userAttributes;
        private readonly ICacheWrapper _cache;
        private readonly IOptions<AppSettings> _settings;
        private readonly IMapper _mapper;

        public AddSightingWizard(LocationClient locations,
                                 FlightClient flights,
                                 AirlineClient airlines,
                                 ManufacturerClient manufacturers,
                                 ModelClient models,
                                 AircraftClient aircraft,
                                 SightingClient sightings,
                                 SightingsSearchClient sightingsSearch,
                                 UserAttributesClient userAttributes,
                                 IOptions<AppSettings> settings,
                                 ICacheWrapper cache,
                                 IMapper mapper)
        {
            _locations = locations;
            _flights = flights;
            _airlines = airlines;
            _manufacturers = manufacturers;
            _models = models;
            _aircraft = aircraft;
            _sightings = sightings;
            _sightingsSearch = sightingsSearch;
            _userAttributes = userAttributes;
            _settings = settings;
            _cache = cache;
            _mapper = mapper;
        }

        /// <summary>
        /// Return the available locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
            => await _locations.GetLocationsAsync(1, int.MaxValue);

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
            => await _manufacturers.GetManufacturersAsync(1, int.MaxValue);

        /// <summary>
        /// Return the available models for a specified manufacturer
        /// </summary>
        /// <returns></returns>
        public async Task<List<Model>> GetModelsAsync(int manufacturerId)
            => await _models.GetModelsAsync(manufacturerId);

        /// <summary>
        /// Return the details of the flight with the specified Id
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightAsync(int flightId)
            => await _flights.GetFlightByIdAsync(flightId);

        /// <summary>
        /// Retrieve or construct the sighting details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SightingDetailsViewModel> GetSightingDetailsModelAsync(string userName, int? sightingId)
        {
            // Retrieve the model from the cache
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            if ((model == null) || (model.SightingId != sightingId))
            {
                // Not cached or the ID has changed, so create a new one and set the "last sighting added" message
                string lastAdded = GetLastSightingAddedMessage(userName);
                ClearCachedLastSightingAddedMessage(userName);

                // If an existing sighting is specified, construct the model using its
                // details
                if (sightingId != null)
                {
                    Sighting sighting = await _sightings.GetSightingAsync(sightingId ?? 0);
                    model = new SightingDetailsViewModel
                    {
                        SightingId = sightingId,
                        LastSightingAddedMessage = lastAdded,
                        Altitude = sighting.Altitude,
                        Date = sighting.Date,
                        FlightNumber = sighting.Flight.Number,
                        LocationId = sighting.LocationId,
                        Registration = sighting.Aircraft.Registration
                    };
                }
                else
                {
                    model = new SightingDetailsViewModel
                    {
                        LastSightingAddedMessage = lastAdded,
                        Date = GetDefaultDate(userName),
                        LocationId = await GetDefaultLocationId(userName)
                    };
                }
            }

            // Set the available locations
            List<Location> locations = await GetLocationsAsync();
            model.SetLocations(locations);

            return model;
        }

        /// <summary>
        /// Get the sighting ID for the current sighting being edited, which will
        /// be null for new sightings
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int? GetCurrentSightingId(string userName)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            return model?.SightingId;
        }

        /// <summary>
        /// Retrieve or constuct the flight details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<FlightDetailsViewModel> GetFlightDetailsModelAsync(string userName)
        {
            // Get the current sighting details
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);

            // Retrieve the model from the cache
            key = GetCacheKey(FlightDetailsKeyPrefix,userName);
            FlightDetailsViewModel model = _cache.Get<FlightDetailsViewModel>(key);
            if (model == null)
            {
                // Not cached, so create a new one, using the cached sighting details
                // model to supply the flight number
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

            // See if this is a potential duplicate - only need to return the first page with 1 result to do the
            // duplicate check
            var duplicates = await _sightingsSearch.GetSightingsByFlightAndDate((DateTime)sighting.Date, sighting.FlightNumber, 1, 1);
            model.IsDuplicate = duplicates?.Count > 0;

            return model;
        }

        /// <summary>
        /// Retrieve or constuct the flight details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<AircraftDetailsViewModel> GetAircraftDetailsModelAsync(string userName)
        {
            // Retrieve the model from the cache
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            AircraftDetailsViewModel model = _cache.Get<AircraftDetailsViewModel>(key);
            if (model == null)
            {
                // Not cached, so create a new one, using the cached sighting details
                // model to supply the aircraft registration
                key = GetCacheKey(SightingDetailsKeyPrefix, userName);
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
                List<Model> models = await GetModelsAsync(model.ManufacturerId ?? 0);
                model.SetModels(models);
            }

            return model;
        }

        /// <summary>
        /// Construct the model to confirm sighting details
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ConfirmDetailsViewModel> GetConfirmDetailsModelAsync(string userName)
        {
            // Get the sighting, flight details and aircraft models and map them
            // into the confirm details model
            ConfirmDetailsViewModel model = new ConfirmDetailsViewModel();

            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);
            _mapper.Map<SightingDetailsViewModel, ConfirmDetailsViewModel>(sighting, model);

            key = GetCacheKey(FlightDetailsKeyPrefix, userName);
            FlightDetailsViewModel flight = _cache.Get<FlightDetailsViewModel>(key);
            _mapper.Map<FlightDetailsViewModel, ConfirmDetailsViewModel>(flight, model);

            key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            AircraftDetailsViewModel aircraft = _cache.Get<AircraftDetailsViewModel>(key);
            _mapper.Map<AircraftDetailsViewModel, ConfirmDetailsViewModel>(aircraft, model);

            // For the location, if we have a new location specified then use that as the
            // location. Otherwise, look up the location by its ID
            if (sighting.LocationId == 0)
            {
                model.Location = sighting.NewLocation;
            }
            else
            {
                Location location = await _locations.GetLocationAsync(sighting.LocationId);
                model.Location = location.Name;
            }

            // Repeat the above logic for the airline
            if (flight.AirlineId == 0)
            {
                model.Airline = flight.NewAirline;
            }
            else
            {
                Airline airline = await _airlines.GetAirlineAsync(flight.AirlineId);
                model.Airline = airline.Name;
            }

            // Repeat the above logic for the manufacturer
            if ((aircraft.ManufacturerId ?? 0) == 0)
            {
                model.Manufacturer = aircraft.NewManufacturer;
            }
            else
            {
                int manufacturerId = aircraft.ManufacturerId ?? 0;
                Manufacturer manufacturer = await _manufacturers.GetManufacturerAsync(manufacturerId);
                model.Manufacturer = manufacturer.Name;
            }

            // Repeat the above logic for the model
            if ((aircraft.ModelId ?? 0) == 0)
            {
                model.Model = aircraft.NewModel;
            }
            else
            {
                int modelId = aircraft.ModelId ?? 0;
                Model aircraftModel = await _models.GetModelAsync(modelId);
                model.Model = aircraftModel.Name;
            }

            return model;
        }

        /// <summary>
        /// Retrieve the message giving the details of the last sighting added
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetLastSightingAddedMessage(string userName)
        {
            string key = GetCacheKey(LastSightingAddedKeyPrefix, userName);
            return _cache.Get<string>(key);
        }

        /// <summary>
        /// Cache the sighting details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheSightingDetailsModel(SightingDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            _cache.Set<SightingDetailsViewModel>(key, model, _settings.Value.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the flight details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheFlightDetailsModel(FlightDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(FlightDetailsKeyPrefix, userName);
            _cache.Set<FlightDetailsViewModel>(key, model, _settings.Value.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the aircraft details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheAircraftDetailsModel(AircraftDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            _cache.Set<AircraftDetailsViewModel>(key, model, _settings.Value.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Clear the cached sighting details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedSightingDetailsModel(string userName)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached flight details model
        /// </summary>
        /// <permission cref=">"
        public void ClearCachedFlightDetailsModel(string userName)
        {
            string key = GetCacheKey(FlightDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached flight details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedAircraftDetailsModel(string userName)
        {
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the last sighting added message from the cache
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedLastSightingAddedMessage(string userName)
        {
            string key = GetCacheKey(LastSightingAddedKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached location
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedLocation(string userName)
        {
            string key = GetCacheKey(DefaultLocationKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Create a new sighting
        /// </summary>
        /// <param name="userName"></param>
        public async Task<Sighting> CreateSighting(string userName)
        {
            Sighting sighting = null;

            // Clear the last sighting added message
            ClearCachedLastSightingAddedMessage(userName);

            // Retrieve the sighting details from the cache
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel details = _cache.Get<SightingDetailsViewModel>(key);
            if (details != null)
            {
                // Create the aircraft and flight, first
                Aircraft aircraft = await RetrieveOrCreateAircraft(userName);
                Flight flight = await RetrieveOrCreateFlight(userName);

                // Create the location, if required
                if (details.LocationId == 0)
                {
                    Location location = await _locations.AddLocationAsync(details.NewLocation);
                    details.LocationId = location.Id;
                }

                // If an existing sighting is being edited, then update it. Otherwise, create
                // a new one
                string message;
                if (details.SightingId != null)
                {
                    sighting = await _sightings.UpdateSightingAsync(details.SightingId ?? 0, details.Date ?? DateTime.Now, details.Altitude ?? 0, aircraft.Id, flight.Id, details.LocationId);
                    message = BuildSightingMessage(sighting, true);
                }
                else
                {
                    sighting = await _sightings.AddSightingAsync(details.Date ?? DateTime.Now, details.Altitude ?? 0, aircraft.Id, flight.Id, details.LocationId);
                    message = BuildSightingMessage(sighting, false);
                }

                // Cache the message giving its details and other properties that are
                // cached to improve data entry speed
                key = GetCacheKey(LastSightingAddedKeyPrefix, userName);
                _cache.Set<string>(key, message, _settings.Value.CacheLifetimeSeconds);

                key = GetCacheKey(DefaultDateKeyPrefix, userName);
                _cache.Set<DateTime>(key, sighting.Date, _settings.Value.CacheLifetimeSeconds);

                key = GetCacheKey(DefaultLocationKeyPrefix, userName);
                _cache.Set<int>(key, sighting.LocationId, _settings.Value.CacheLifetimeSeconds);
            }

            // Clear the cached data
            Reset(userName);

            return sighting;
        }

        /// <summary>
        /// Reset the wizard by clearing the cached details models
        /// </summary>
        /// <param name="userName"></param>
        public void Reset(string userName)
        {
            ClearCachedSightingDetailsModel(userName);
            ClearCachedFlightDetailsModel(userName);
            ClearCachedAircraftDetailsModel(userName);
        }

        /// <summary>
        /// Build the message to report addition/update of a sighting
        /// </summary>
        /// <param name="sighting"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        private static string BuildSightingMessage(Sighting sighting, bool isUpdate)
        {
            StringBuilder builder = new();
            builder.Append("Your sighting of flight ");
            builder.Append(sighting.Flight.Number);
            builder.Append(", aircraft ");
            builder.Append(sighting.Aircraft.Registration);

            if (sighting.Aircraft.Model != null)
            {
                builder.Append("(");
                builder.Append(sighting.Aircraft.Model.Manufacturer.Name);
                builder.Append(" ");
                builder.Append(sighting.Aircraft.Model.Name);
                builder.Append(")");
            }

            builder.Append(", at ");
            builder.Append(sighting.Location.Name);
            builder.Append(" on ");
            builder.Append(sighting.Date.ToString("dd-MMM-yyyy"));

            if (isUpdate)
            {
                builder.Append(" has been updated");
            }
            else
            {
                builder.Append(" has been added to the database");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Either retrieve an existing aircraft or create a new one
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<Aircraft> RetrieveOrCreateAircraft(string userName)
        {
            Aircraft aircraft = null;

            // Retrieve the aircraft details from the cache
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            AircraftDetailsViewModel details = _cache.Get<AircraftDetailsViewModel>(key);
            if (details != null)
            {
                // If this is a sighting for an existing aircraft, just return it.
                // Otherwise, we need to create a new aircraft
                if (details.AircraftId > 0)
                {
                    aircraft = await _aircraft.GetAircraftByIdAsync(details.AircraftId ?? 0);
                }
                else
                {
                    // If an existing manufacturer's not been specified and we have a manufacturer name,
                    // create a new manufacturer
                    if (((details.ManufacturerId ?? 0) == 0) && !string.IsNullOrEmpty(details.NewManufacturer))
                    {
                        // With no manufacturer selected, we're creating a new manufacturer and model
                        Manufacturer manufacturer = await _manufacturers.AddManufacturerAsync(details.NewManufacturer);
                        details.ManufacturerId = manufacturer.Id;

                        // Must be creating a new model, so NULL the model ID on the details
                        details.ModelId = null;
                    }

                    // If an existing model's not been specified and we have a model name and a manufacturer,
                    // create a newmodel
                    if (((details.ModelId ?? 0) == 0) && (details.ManufacturerId > 0) && !string.IsNullOrEmpty(details.NewModel))
                    {
                        // With no model selected, we're creating a new model for the selected manufacturer
                        Model model = await _models.AddModelAsync(details.NewModel, details.ManufacturerId ?? 0);
                        details.ModelId = model.Id;
                    }

                    // Create the aircraft
                    int? manufactured = (details.Age != null) ? DateTime.Now.Year - details.Age : null;
                    aircraft = await _aircraft.AddAircraftAsync(details.Registration, details.SerialNumber, manufactured, details.ModelId ?? 0);
                }
            }

            return aircraft;
        }

        /// <summary>
        /// Either retrieve an existing flight or create a new one
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<Flight> RetrieveOrCreateFlight(string userName)
        {
            Flight flight = null;

            // Retrieve the flight details from the cache
            string key = GetCacheKey(FlightDetailsKeyPrefix, userName);
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
        /// The sighting date is cached between entries to speed up multiple
        /// entries on the same date. If it's not been cached yet, it defaults
        /// to today
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private DateTime GetDefaultDate(string userName)
        {
            string key = GetCacheKey(DefaultDateKeyPrefix, userName);
            DateTime? defaultDate = _cache.Get<DateTime?>(key);
            return defaultDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        /// <summary>
        /// The sighting location is cached between entries to speed up multiple entries at the same location.
        /// There is also an (optional) default location per user that is used if there's no current selected
        /// location
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<int> GetDefaultLocationId(string userName)
        {
            string key = GetCacheKey(DefaultLocationKeyPrefix, userName);
            int? locationId = _cache.Get<int?>(key);
            if (locationId == null)
            {
                await _userAttributes.GetUserAttributesAsync(userName, true);
                var defaultUserLocation = _userAttributes.GetCachedUserAttribute(_settings.Value.DefaultLocationAttribute);
                if (int.TryParse(defaultUserLocation.Value, out int defaultUserLocationId))
                {
                    locationId = defaultUserLocationId;
                }
            }
            return locationId ?? 0;
        }

        /// <summary>
        /// Construct a key for caching data
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static string GetCacheKey(string prefix, string userName)
        {
            string key = $"{prefix}.{userName}";
            return key;
        }
    }
}
