using FlightRecorder.Entities.Db;

namespace FlightRecorder.Mvc.Models
{
    public class CountryListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<Country> Countries { get; private set; }

        /// <summary>
        /// Set the list of countries to be exposed by this view model
        /// </summary>
        /// <param name="countries"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetCountries(IEnumerable<Country> countries, int pageNumber, int pageSize)
        {
            Countries = countries ?? [];
            PageNumber = pageNumber;
            SetPreviousNextEnabled(Countries.Count(), pageNumber, pageSize);
        }
    }
}
