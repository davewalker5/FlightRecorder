using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IFlightClient
    {
        Task<List<Flight>> GetFlightsByRouteAsync(string embarkation, string destination);
        Task<List<Flight>> GetFlightsByAirlineAsync(long airlineId);
        Task<List<Flight>> GetFlightsByNumberAsync(string number);
        Task<Flight> GetFlightByIdAsync(long id);
        Task<Flight> AddFlightAsync(string number, string embarkation, string destination, long airlineId);
        Task<Flight> UpdateFlightAsync(long flightId, string number, string embarkation, string destination, long airlineId);
    }
}
