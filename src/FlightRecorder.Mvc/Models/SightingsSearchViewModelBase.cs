using System.Collections.Generic;
using FlightRecorder.Mvc.Entities;

namespace FlightRecorder.Mvc.Models
{
    public abstract class SightingsSearchViewModelBase
    {
        public List<Sighting> Sightings { get; private set; }
        public int PageNumber { get; set; }
        public bool PreviousEnabled { get; private set; }
        public bool NextEnabled { get; private set; }
        public string Action { get; set; }
        public bool HasNoMatchingResults { get; set; }

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
            SetPreviousNextEnabled(pageNumber, pageSize);
        }

        /// <summary>
        /// Set the "previous/next" button enabled flags according to the
        /// following logic, where SZ is the page size:
        ///
        /// Flight  Page    Previous    Next
        /// Count   Number  Enabled     Enabled
        ///
        /// 0       -       No          No
        /// = SZ    1       No          Yes   
        /// < SZ    1       No          No
        /// = SZ    > 1     Yes         Yes
        /// < SZ    > 1     Yes         No
        /// 
        /// </summary>
        /// <param name="pageNumber"/>
        /// <param name="pageSize"/>
        /// <returns></returns>
        private void SetPreviousNextEnabled(int pageNumber, int pageSize)
        {
            int count = Sightings?.Count ?? 0;
            PreviousEnabled = (pageNumber > 1);
            NextEnabled = (count == pageSize);
        }
    }
}
