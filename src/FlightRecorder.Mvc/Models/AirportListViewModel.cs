using FlightRecorder.Mvc.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FlightRecorder.Mvc.Models
{
    public class AirportListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<Airport> Airports { get; private set; }

        /// <summary>
        /// Set the list of airports to be exposed by this view model
        /// </summary>
        /// <param name="airports"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetAirports(IEnumerable<Airport> airports, int pageNumber, int pageSize)
        {
            Airports = airports;
            PageNumber = pageNumber;
            SetPreviousNextEnabled(airports.Count(), pageNumber, pageSize);
        }
    }
}