using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface ILocationManager : IManagerBase<Location>
    {
        Location Add(string name);
    }
}