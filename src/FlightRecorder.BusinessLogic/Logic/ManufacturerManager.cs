using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class ManufacturerManager : ManagerBase<Manufacturer>, IManufacturerManager
    {
        internal ManufacturerManager(FlightRecorderDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Add a named manufacturer, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Manufacturer Add(string name)
        {
            Manufacturer manufacturer = Get(a => a.Name == name);

            if (manufacturer == null)
            {
                manufacturer = Add(new Manufacturer { Name = name });
            }

            return manufacturer;
        }
    }
}
