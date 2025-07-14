using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public class LocationListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<Location> Locations { get; private set; }

        /// <summary>
        /// Set the list of locations to be exposed by this view model
        /// </summary>
        /// <param name="locations"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetLocations(IEnumerable<Location> locations, int pageNumber, int pageSize)
        {
            Locations = locations ?? [];
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Locations.Count(), pageNumber, pageSize);
        }
    }
}