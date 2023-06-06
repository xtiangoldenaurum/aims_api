using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IAccessRightRepository
    {
        Task<IEnumerable<AccessRightModel>> GetAllAccessRight();
        Task<IEnumerable<AccessRightModel>> GetAccessRightPg(int pageNum, int pageItem);
        Task<AccessRightPagedMdl?> GetAccessRightPaged(int pageNum, int pageItem);
        Task<IEnumerable<AccessRightModel>> GetAccessRightPgSrch(string searchKey, int pageNum, int pageItem);
        Task<AccessRightPagedMdl?> GetAccessRightSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<AccessRightModel> GetAccessRightById(string accessRightId);
        Task<bool> CreateAccessRight(AccessRightModel accessRight, IEnumerable<AccessRightDetailModel> details);
        Task<bool> CreateAccessRightHeader(AccessRightModel accessRight);
        Task<bool> AccessRightExists(string accessRightId);
        Task<bool> UpdateAccessRight(AccessRightModel accessRight, IEnumerable<AccessRightDetailModel> details);
        Task<bool> UpdateAccessRight(AccessRightModel accessRight, IEnumerable<AccessRightDetailModelMod> details);
        Task<bool> AccessRightInUse(string accessRightId);
        Task<bool> DeleteAccessRight(string accessRightId, string userAccountId);
    }
}
