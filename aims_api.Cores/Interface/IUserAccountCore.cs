using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IUserAccountCore
    {
        Task<RequestResponse> GetUserAccountsSpecial(UserAccountFilterMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetUserAccountPg(int pageNum, int pageItem);
        Task<RequestResponse> GetUserAccPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetActiveUserAccountPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInActiveUserAccountPg(int pageNum, int pageItem);
        Task<RequestResponse> GetUserAccountPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetUserAccSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetUserAccountById(string userAccountId);
        Task<RequestResponse> GetUserAccountPgFiltered(UserAccountFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> GetUserAccFltrPaged(UserAccountFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> GetUsrAccByAccessRightIdPg(string accessRightId, int pageNum, int pageItem);
        Task<RequestResponse> CreateUserAccount(UserAccountModelMod userAccount);
        Task<RequestResponse> UpdateUserAccount(UserAccountModelMod userAccount);
        Task<RequestResponse> DeleteUserAccount(string delUserAccountId, string userAccountId);
        Task<RequestResponse> DeActUserAccount(string delUserAccountId, string userAccountId);
        Task<RequestResponse> ReActUserAccount(string delUserAccountId, string userAccountId);
        Task<RequestResponse> UserAccountValidationWeb(LoginCredentailsModel account);
        Task<RequestResponse> UserAccountValidationMob(LoginCredentailsModel account);
    }
}
