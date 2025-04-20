using FlightRecorder.Entities.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirportsApi
    {
        Task<Dictionary<ApiPropertyType, string>> LookupAirportByIATACode(string code);
    }
}