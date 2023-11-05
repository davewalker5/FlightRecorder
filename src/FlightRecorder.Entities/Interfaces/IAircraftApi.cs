using FlightRecorder.Entities.Api;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Api.AeroDataBox
{
    public interface IAircraftApi
    {
        Task<Dictionary<ApiPropertyType, string>> LookupAircraftByICAOAddress(string address);
        Task<Dictionary<ApiPropertyType, string>> LookupAircraftByRegistration(string registration);
    }
}