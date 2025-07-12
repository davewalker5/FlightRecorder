using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IModelClient
    {
        Task<List<Model>> GetModelsAsync(long manufacturerId);
        Task<Model> GetModelAsync(long id);
        Task<Model> AddModelAsync(string name, long manufacturerId);
        Task<Model> UpdateModelAsync(long id, long manufacturerId, string name);
    }
}
