using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;

namespace FlightRecorder.Client.Interfaces
{
    public interface IUserAttributesClient
    {
        Task<List<UserAttributeValue>> GetUserAttributesAsync(string userName, bool useCachedValue);
        UserAttributeValue GetCachedUserAttribute(string attributeName);
    }
}
