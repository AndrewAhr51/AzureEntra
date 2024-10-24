
using EntraIdBL.Models;

namespace EntraIdBL.Interfaces
{
    public interface IGraphClient
    {
        public Task<List<EntraIdRecord>> GetGroupMembers(string _region, string _groupId);
    }
}
