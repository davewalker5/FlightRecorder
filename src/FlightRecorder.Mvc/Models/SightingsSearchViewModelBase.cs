using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public abstract class SightingsSearchViewModelBase : PaginatedViewModelBase
    {
        public List<Sighting> Sightings { get; private set; }

        /// <summary>
        /// Set the collection of sightings that are exposed to the view
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetSightings(List<Sighting> sightings, int pageNumber, int pageSize)
        {
            Sightings = sightings;
            HasNoMatchingResults = (sightings == null);
            PageNumber = pageNumber;
            SetPreviousNextEnabled(sightings?.Count ?? 0, pageNumber, pageSize);
        }
    }
}
