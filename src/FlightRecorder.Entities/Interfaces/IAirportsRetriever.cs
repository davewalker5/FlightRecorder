using FlightRecorder.Entities.Db;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirportsRetriever
    {
        Task<List<Airport>> GetAirportsAsync(int pageNumber, int pageSize);
    }
}