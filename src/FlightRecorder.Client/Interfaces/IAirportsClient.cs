using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.Client.Interfaces
{
    public interface IAirportsClient : IAirportsRetriever
    {
        Task<List<Airport>> GetAirportsByCodeAsync(string code);
        Task<Airport> GetAirportByIdAsync(long id);
        Task<Airport> AddAirportAsync(string code, string name, long countryId);
        Task<Airport> UpdateAirportAsync(long airportId, string code, string name, long countryId);
        Task DeleteAirportAsync(long id);
    }
}
