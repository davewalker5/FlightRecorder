using System.ComponentModel;

namespace FlightRecorder.Mvc.Models
{
    public abstract class DateBasedReportViewModelBase<T> : PaginatedViewModelBase where T : class
    {
        public List<T> Records { get; private set; }

        [DisplayName("From")]
        public string From { get; set; }

        [DisplayName("To")]
        public string To { get; set; }

        /// <summary>
        /// Set the collection of records that are exposed to the view
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetRecords(List<T> records, int pageNumber, int pageSize)
        {
            Records = records;
            HasNoMatchingResults = (records == null);
            PageNumber = pageNumber;
            SetPreviousNextEnabled(records?.Count ?? 0, pageNumber, pageSize);
        }
    }
}
