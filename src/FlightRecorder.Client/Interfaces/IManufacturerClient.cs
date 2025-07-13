using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IManufacturerClient
    {
        Task<List<Manufacturer>> GetManufacturersAsync(int pageNumber, int pageSize);
        Task<Manufacturer> GetManufacturerAsync(long id);
        Task<Manufacturer> AddManufacturerAsync(string name);
        Task<Manufacturer> UpdateManufacturerAsync(long id, string name);
        Task DeleteManufacturerAsync(long id);
    }
}
