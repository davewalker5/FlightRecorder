using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IAirlineClient
    {
        Task<List<Airline>> GetAirlinesAsync();
        Task<Airline> GetAirlineAsync(long id);
        Task<Airline> AddAirlineAsync(string name);
        Task<Airline> UpdateAirlineAsync(long id, string name);
    }
}
