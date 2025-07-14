using System;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface ISightingClient
    {
        Task<Sighting> GetSightingAsync(long id);
        Task<Sighting> AddSightingAsync(DateTime date, long altitude, long aircraftId, long flightId, long locationId, bool isMyFlight);
        Task<Sighting> UpdateSightingAsync(long id, DateTime date, long altitude, long aircraftId, long flightId, long locationId, bool isMyFlight);
        Task DeleteSightingAsync(long id);
    }
}
