using FlightRecorder.Entities.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IFlightsApi
    {
        Task<Dictionary<ApiPropertyType, string>> LookupFlightByNumber(string number);
        Task<Dictionary<ApiPropertyType, string>> LookupFlightByNumberAndDate(string number, DateTime date);
    }
}