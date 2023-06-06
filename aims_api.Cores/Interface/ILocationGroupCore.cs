using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface ILocationGroupCore
    {
        Task<RequestResponse> GetLocationGroupPg(int pageNum, int pageItem);
        Task<RequestResponse> GetLocationGroupPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetLocationGroupById(string locationGroupId);
        Task<RequestResponse> CreateLocationGroup(LocationGroupModel locationGroup);
        Task<RequestResponse> UpdateLocationGroup(LocationGroupModel locationGroup);
        Task<RequestResponse> DeleteLocationGroup(string locationGroupId, string userAccountId);
    }
}
