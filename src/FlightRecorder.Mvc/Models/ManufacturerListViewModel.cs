using FlightRecorder.Entities.Db;
using System.Collections.Generic;
using System.Linq;

namespace FlightRecorder.Mvc.Models
{
    public class ManufacturerListViewModel : PaginatedViewModelBase
    {
        public IEnumerable<Manufacturer> Manufacturers { get; private set; }

        /// <summary>
        /// Set the list of manufacturers to be exposed by this view model
        /// </summary>
        /// <param name="manufacturers"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public void SetManufacturers(IEnumerable<Manufacturer> manufacturers, int pageNumber, int pageSize)
        {
            Manufacturers = manufacturers;
            PageNumber = pageNumber;
            SetPreviousNextEnabled(manufacturers.Count(), pageNumber, pageSize);
        }
    }
}