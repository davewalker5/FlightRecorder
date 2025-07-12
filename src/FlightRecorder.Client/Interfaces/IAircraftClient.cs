using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IAircraftClient
    {
        Task<List<Aircraft>> GetAircraftByModelAsync(long modelId);
        Task<Aircraft> GetAircraftByRegistrationAsync(string registration);
        Task<Aircraft> GetAircraftByIdAsync(long id);
        Task<Aircraft> AddAircraftAsync(string registration, string serialNumber, long? yearOfManufacture, long? modelId);
        Task<Aircraft> UpdateAircraftAsync(long id, string registration, string serialNumber, int? yearOfManufacture, long modelId);
    }
}
