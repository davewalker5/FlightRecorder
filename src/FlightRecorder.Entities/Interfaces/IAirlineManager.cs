using FlightRecorder.Entities.Db;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirlineManager : IManagerBase<Airline>
    {
        Airline Add(string name);
    }
}