using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface ISightingsSearchClient
    {
        Task<List<Sighting>> GetSightingsByRoute(string embarkation, string destination, int page, int pageSize);
        Task<List<Sighting>> GetSightingsByFlight(string number, int page, int pageSize);
        Task<Sighting> GetMostRecentFlightSighting(string flightNumber);
        Task<Sighting> GetMostRecentAircraftSighting(string registration);
        Task<List<Sighting>> GetSightingsByAirline(long airlineId, int page, int pageSize);
        Task<List<Sighting>> GetSightingsByAircraft(long aircraftId, int page, int pageSize);
        Task<List<Sighting>> GetSightingsByDate(DateTime from, DateTime to, int page, int pageSize);
    }
}
