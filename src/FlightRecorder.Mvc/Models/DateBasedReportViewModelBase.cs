using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightRecorder.Mvc.Models
{
    public abstract class DateBasedReportViewModelBase<T> : PaginatedViewModelBase where T : class
    {
        public List<T> Records { get; private set; }

        [DisplayName("From")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        public DateTime? From { get; set; }

        [DisplayName("To")]
        [UIHint("DatePicker")]
        [DataType(DataType.Date)]
        public DateTime? To { get; set; }

        /// <summary>
        /// Set the collection of records that are exposed to the view
        /// </summary>
        /// <param name="sightings"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetRecords(List<T> records, int pageNumber, int pageSize)
        {
            Records = records ?? [];
            HasNoMatchingResults = !Records.Any();
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Records.Count, pageNumber, pageSize);
        }
    }
}
