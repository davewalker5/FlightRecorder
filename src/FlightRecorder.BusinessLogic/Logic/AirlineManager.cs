using FlightRecorder.BusinessLogic.Base;
using FlightRecorder.Data;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Interfaces;

namespace FlightRecorder.BusinessLogic.Logic
{
    internal class AirlineManager : ManagerBase<Airline>, IAirlineManager
    {
        internal AirlineManager(FlightRecorderDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Add a named airline, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Airline Add(string name)
        {
            Airline airline = Get(a => a.Name == name);

            if (airline == null)
            {
                airline = Add(new Airline { Name = name });
            }

            return airline;
        }
    }
}
