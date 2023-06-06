using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IAccessRightCore
    {
        Task<RequestResponse> GetAccessRightsSpecial(string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAllAccessRight();
        Task<RequestResponse> GetAccessRightPg(int pageNum, int pageItem);
        Task<RequestResponse> GetAccessRightPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetAccessRightPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAccessRightSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetAccessRightById(string accessRightId);
        Task<RequestResponse> CreateAccessRightHeader(AccessRightModel accessRight);
        Task<RequestResponse> UpdateAccessRight(AccessRightHDModel accessRightHD);
        Task<RequestResponse> DeleteAccessRight(string accessRightId, string userAccountId);
    }
}
