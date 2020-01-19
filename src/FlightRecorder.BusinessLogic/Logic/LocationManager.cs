using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class LocationManager : ManagerBase<Location>, ILocationManager
    {
        internal LocationManager(FlightRecorderDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Add a named location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Location Add(string name)
        {
            Location location = Get(a => a.Name == name);

            if (location == null)
            {
                location = Add(new Location { Name = name });
            }

            return location;
        }
    }
}
