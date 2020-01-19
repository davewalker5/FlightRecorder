using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IManufacturerManager : IManagerBase<Manufacturer>
    {
        Manufacturer Add(string name);
    }
}