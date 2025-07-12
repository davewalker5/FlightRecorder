using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface ILocationClient
    {
        Task<List<Location>> GetLocationsAsync(int pageNumber, int pageSize);
        Task<Location> GetLocationAsync(long id);
        Task<Location> AddLocationAsync(string name);
        Task<Location> UpdateLocationAsync(long id, string name);
    }
}
