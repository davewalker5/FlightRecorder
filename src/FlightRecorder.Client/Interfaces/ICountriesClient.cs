using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface ICountriesClient
    {
        Task<List<Country>> GetCountriesAsync(int pageNumber, int pageSize);
        Task<Country> GetCountryAsync(long id);
        Task<Country> AddCountryAsync(string name);
        Task<Country> UpdateCountryAsync(long id, string name);
        Task DeleteCountryAsync(long id);
    }
}
