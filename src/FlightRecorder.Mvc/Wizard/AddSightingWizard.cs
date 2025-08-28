using AutoMapper;
using FlightRecorder.Entities.Db;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Mvc.Models;
using System.Text;
using FlightRecorder.Entities.Config;

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

        private readonly ILocationClient _locations;
        private readonly IFlightClient _flights;
        private readonly IAirlineClient _airlines;
        private readonly IManufacturerClient _manufacturers;
        private readonly IModelClient _models;
        private readonly IAircraftClient _aircraft;
        private readonly ISightingClient _sightings;
        private readonly ISightingsSearchClient _sightingsSearch;
        private readonly IUserAttributesClient _userAttributes;
        private readonly ICacheWrapper _cache;
        private readonly FlightRecorderApplicationSettings _settings;
        private readonly IMapper _mapper;
        private readonly ILogger<AddSightingWizard> _logger;

        public AddSightingWizard(ILocationClient locations,
                                 IFlightClient flights,
                                 IAirlineClient airlines,
                                 IManufacturerClient manufacturers,
                                 IModelClient models,
                                 IAircraftClient aircraft,
                                 ISightingClient sightings,
                                 ISightingsSearchClient sightingsSearch,
                                 IUserAttributesClient userAttributes,
                                 FlightRecorderApplicationSettings settings,
                                 ICacheWrapper cache,
                                 IMapper mapper,
                                 ILogger<AddSightingWizard> logger)
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
            _logger = logger;
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
        public async Task<List<Model>> GetModelsAsync(long manufacturerId)
            => await _models.GetModelsAsync(manufacturerId);

        /// <summary>
        /// Return the details of the flight with the specified Id
        /// </summary>
        /// <param name="flightId"></param>
        /// <returns></returns>
        public async Task<Flight> GetFlightAsync(long flightId)
            => await _flights.GetFlightByIdAsync(flightId);

        /// <summary>
        /// Retrieve or construct the sighting details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SightingDetailsViewModel> GetSightingDetailsModelAsync(string userName, long? sightingId)
        {
            _logger.LogDebug($"Resolving sighting details model for sighting ID {sightingId} for user {userName}");

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
                    _logger.LogDebug($"Creating new sighting details model");
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

            _logger.LogDebug($"Resolved sighting details model: {model}");

            return model;
        }

        /// <summary>
        /// Get the sighting ID for the current sighting being edited, which will
        /// be null for new sightings
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public long? GetCurrentSightingId(string userName)
        {
            _logger.LogDebug($"Retrieving current sighting ID for user {userName}");
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            _logger.LogDebug($"Current sighting ID for user {userName} = {model?.SightingId}");
            return model?.SightingId;
        }

        /// <summary>
        /// Retrieve or constuct the flight details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<FlightDetailsViewModel> GetFlightDetailsModelAsync(string userName)
        {
            _logger.LogDebug($"Resolving flight details model for user {userName}");

            // Get the current sighting details
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);

            // Retrieve the model from the cache
            key = GetCacheKey(FlightDetailsKeyPrefix,userName);
            FlightDetailsViewModel model = _cache.Get<FlightDetailsViewModel>(key);
            if (model == null)
            {
                _logger.LogDebug($"Creating new flight details model");

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
                _logger.LogDebug($"Adding existing flight details to model: {flight}");
                model.FlightId = flight.Id;
                model.Embarkation = flight.Embarkation;
                model.Destination = flight.Destination;
                model.AirlineId = flight.AirlineId;

                // Retrive the most recent sighting of this flight and see if this is a duplicate. Note that duplicates
                // are not reported when editing an existing sighting
                model.MostRecentSighting = await _sightingsSearch.GetMostRecentFlightSighting(sighting.FlightNumber);
                model.IsDuplicate = sighting.SightingId > 0 ? false : model.MostRecentSighting?.Date == sighting.Date;
            }

            _logger.LogDebug($"Resolved flight details model: {model}");
            return model;
        }

        /// <summary>
        /// Retrieve or constuct the flight details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<AircraftDetailsViewModel> GetAircraftDetailsModelAsync(string userName)
        {
            _logger.LogDebug($"Resolving aircraft details model for user {userName}");

            // Retrieve the model from the cache
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            AircraftDetailsViewModel model = _cache.Get<AircraftDetailsViewModel>(key);
            if (model == null)
            {
                _logger.LogDebug($"Creating new aircraft details model");

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
            _logger.LogDebug($"Retrieving aircraft with registration {model.Registration}");
            Aircraft aircraft = await _aircraft.GetAircraftByRegistrationAsync(model.Registration);
            _logger.LogDebug($"Retrieved aircraft: {aircraft}");

            if (aircraft != null)
            {
                _logger.LogDebug($"Adding existing aircraft details to model: {aircraft}");

                // It is, so assign the aircraft properties
                model.AircraftId = aircraft.Id;
                model.SerialNumber = aircraft.SerialNumber;
                model.ManufacturerId = aircraft?.Model.ManufacturerId;
                model.ModelId = aircraft.ModelId;
                model.Age = DateTime.Now.Year - aircraft.Manufactured;

                // Load the models for the aircraft's manufacturer
                List<Model> models = await GetModelsAsync(model.ManufacturerId ?? 0);
                model.SetModels(models);

                // Retrive the most recent sighting of this aircraft
                model.MostRecentSighting = await _sightingsSearch.GetMostRecentAircraftSighting(aircraft.Registration);
            }

            _logger.LogDebug($"Resolved aircraft details model: {model}");
            return model;
        }

        /// <summary>
        /// Construct the model to confirm sighting details
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ConfirmDetailsViewModel> GetConfirmDetailsModelAsync(string userName)
        {
            _logger.LogDebug($"Creating confirm details model for user {userName}");

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
                long manufacturerId = aircraft.ManufacturerId ?? 0;
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
                long modelId = aircraft.ModelId ?? 0;
                Model aircraftModel = await _models.GetModelAsync(modelId);
                model.Model = aircraftModel.Name;
            }

            _logger.LogDebug($"Created confirm details model: {model}");
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
            _cache.Set<SightingDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the flight details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheFlightDetailsModel(FlightDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(FlightDetailsKeyPrefix, userName);
            _cache.Set<FlightDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the aircraft details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheAircraftDetailsModel(AircraftDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            _cache.Set<AircraftDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
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

            _logger.LogDebug($"Creating new sighting for user {userName}");

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
                    _logger.LogDebug($"Updating existing sighting with ID {details.SightingId} for user {userName}");

                    sighting = await _sightings.UpdateSightingAsync(
                        details.SightingId ?? 0,
                        details.Date ?? DateTime.Now,
                        details.Altitude ?? 0,
                        aircraft.Id, flight.Id,
                        details.LocationId,
                        details.IsMyFlight);
                    message = BuildSightingMessage(sighting, true);
                }
                else
                {
                    _logger.LogDebug($"Creating new sighting for user {userName}");

                    sighting = await _sightings.AddSightingAsync(
                        details.Date ?? DateTime.Now,
                        details.Altitude ?? 0,
                        aircraft.Id,
                        flight.Id,
                        details.LocationId,
                        details.IsMyFlight);
                    message = BuildSightingMessage(sighting, false);
                }

                // Cache the message giving its details and other properties that are
                // cached to improve data entry speed
                key = GetCacheKey(LastSightingAddedKeyPrefix, userName);
                _cache.Set<string>(key, message, _settings.CacheLifetimeSeconds);

                key = GetCacheKey(DefaultDateKeyPrefix, userName);
                _cache.Set<DateTime>(key, sighting.Date, _settings.CacheLifetimeSeconds);

                key = GetCacheKey(DefaultLocationKeyPrefix, userName);
                _cache.Set<long>(key, sighting.LocationId, _settings.CacheLifetimeSeconds);
            }

            // Clear the cached data
            Reset(userName);

            _logger.LogDebug($"Created sighting: {sighting}");

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

            _logger.LogDebug($"Creating or retrieving aircraft for {userName}");

            // Retrieve the aircraft details from the cache
            string key = GetCacheKey(AircraftDetailsKeyPrefix, userName);
            AircraftDetailsViewModel details = _cache.Get<AircraftDetailsViewModel>(key);
            if (details != null)
            {
                _logger.LogDebug($"Retrieved aircraft details model: {details}");

                // If this is a sighting for an existing aircraft, just return it.
                // Otherwise, we need to create a new aircraft
                if (details.AircraftId > 0)
                {
                    _logger.LogDebug($"Retrieving aircraft with Id: {details.AircraftId}");
                    aircraft = await _aircraft.GetAircraftByIdAsync(details.AircraftId ?? 0);
                }
                else
                {
                    _logger.LogDebug($"Creating new aircraft");

                    // If an existing manufacturer's not been specified and we have a manufacturer name,
                    // create a new manufacturer
                    if (((details.ManufacturerId ?? 0) == 0) && !string.IsNullOrEmpty(details.NewManufacturer))
                    {
                        _logger.LogDebug($"Creating new manufacturer: {details.NewManufacturer}");

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
                        _logger.LogDebug($"Creating new model: {details.NewModel}");

                        // With no model selected, we're creating a new model for the selected manufacturer
                        Model model = await _models.AddModelAsync(details.NewModel, details.ManufacturerId ?? 0);
                        details.ModelId = model.Id;
                    }

                    // Create the aircraft
                    long? manufactured = (details.Age != null) ? DateTime.Now.Year - details.Age : null;
                    aircraft = await _aircraft.AddAircraftAsync(details.Registration, details.SerialNumber, manufactured, details.ModelId ?? 0);
                }
            }

            _logger.LogDebug($"Retrieved or created aircraft: {aircraft}");
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

            _logger.LogDebug($"Creating or retrieving flight for {userName}");

            // Retrieve the flight details from the cache
            string key = GetCacheKey(FlightDetailsKeyPrefix, userName);
            FlightDetailsViewModel details = _cache.Get<FlightDetailsViewModel>(key);
            if (details != null)
            {
                _logger.LogDebug($"Retrieved flight details model: {details}");

                // If this is a sighting for an existing flight, just return it.
                // Otherwise, we need to create a new flight
                if (details.FlightId > 0)
                {
                    _logger.LogDebug($"Retrieving flight with Id: {details.FlightId}");
                    flight = await _flights.GetFlightByIdAsync(details.FlightId);
                }
                else
                {
                    if (details.AirlineId == 0)
                    {
                        _logger.LogDebug($"Creating new model: {details.NewAirline}");

                        // If there's no airline selected, we're creating a new one
                        Airline airline = await _airlines.AddAirlineAsync(details.NewAirline);
                        details.AirlineId = airline.Id;
                    }

                    // Create the flight
                    flight = await _flights.AddFlightAsync(details.FlightNumber, details.Embarkation, details.Destination, details.AirlineId);
                }
            }

            _logger.LogDebug($"Retrieved or created flight: {flight}");
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
        private async Task<long> GetDefaultLocationId(string userName)
        {
            string key = GetCacheKey(DefaultLocationKeyPrefix, userName);
            long? locationId = _cache.Get<long?>(key);
            if (locationId == null)
            {
                await _userAttributes.GetUserAttributesAsync(userName, true);
                var defaultUserLocation = _userAttributes.GetCachedUserAttribute(_settings.DefaultLocationAttribute);
                if (long.TryParse(defaultUserLocation?.Value, out long defaultUserLocationId))
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
