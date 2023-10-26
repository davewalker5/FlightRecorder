using FlightRecorder.Entities.Db;
using System.Collections.Generic;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IAirportExporter
    {
        void Export(IEnumerable<Airport> airports, string file);
    }
}