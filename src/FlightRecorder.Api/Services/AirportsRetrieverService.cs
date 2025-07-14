using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.Api.Services
{
    public class AirportsRetrieverService : IAirportsRetriever
    {
        private readonly FlightRecorderFactory _factory;

        public AirportsRetrieverService(FlightRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Return a list of airports
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<Airport>> GetAirportsAsync(int pageNumber, int pageSize)
            => await _factory.Airports.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
    }
}