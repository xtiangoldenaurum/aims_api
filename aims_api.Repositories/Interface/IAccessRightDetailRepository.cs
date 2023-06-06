using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IAccessRightDetailRepository
    {
        Task<IEnumerable<AccessRightDetailModel>> GetAllAccessRightDetail();
        Task<IEnumerable<AccessRightDetailModelMod>> GetAccessRightDetailById(string accessRightId);
        Task<bool> CheckAccessRightDetailsExists(IDbConnection db, string accessRightId);
        Task<IEnumerable<AccessRightDetailModelMod>> GetUserAccessDetails(string accessRightId);
        Task<IEnumerable<UserAllowedActionsModel>> GetUserAllowedActions(string accessRightId);
        Task<IEnumerable<UserAllowedActionsModel>> GetUserAllowedActionsWeb(string accessRightId);
        Task<IEnumerable<UserAllowedActionsModel>> GetUserAllowedActionsMob(string accessRightId);
        Task<bool> AccessRightDetailExists(AccessRightDetailModel detail);
        Task<bool> CreateAccessRightDetail(IDbConnection db, string accessrightId, IEnumerable<AccessRightDetailModel> details);
        Task<bool> CreateAccessRightDetail(IDbConnection db, string accessrightId, string createdBy, string modifiedBy, IEnumerable<AccessRightDetailModelMod> details);
        Task<bool> DeletePrevAccessRightDetails(IDbConnection db, string accessRightId);
    }
}
