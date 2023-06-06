using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface ILocationGroupRepository
    {
        Task<IEnumerable<LocationGroupModel>> GetLocationGroupPg(int pageNum, int pageItem);
        Task<IEnumerable<LocationGroupModel>> GetLocationGroupPgSrch(string searchKey, int pageNum, int pageItem);
        Task<LocationGroupModel> GetLocationGroupById(string locationGroupId);
        Task<bool> LocationGroupExists(string locationGroupId);
        Task<bool> CreateLocationGroup(LocationGroupModel locationGroup);
        Task<bool> UpdateLocationGroup(LocationGroupModel locationGroup);
        Task<bool> DeleteLocationGroup(string locationGroupId, string userAccountId);
    }
}
