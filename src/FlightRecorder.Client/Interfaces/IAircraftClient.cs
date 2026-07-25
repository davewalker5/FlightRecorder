using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IAircraftClient
    {
        Task<List<Aircraft>> GetAircraftByModelAsync(long modelId);
        Task<Aircraft> GetAircraftByAddressAsync(string address);
        Task<Aircraft> GetAircraftByRegistrationAsync(string registration);
        Task<Aircraft> GetAircraftByIdAsync(long id);
        Task<Aircraft> AddAircraftAsync(string address, string registration, string serialNumber, long? yearOfManufacture, long? modelId);
        Task<Aircraft> UpdateAircraftAsync(long id, string address, string registration, string serialNumber, int? yearOfManufacture, long? modelId);
        Task DeleteAircraftAsync(long id);
    }
}
