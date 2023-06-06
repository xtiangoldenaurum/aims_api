using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IUserAccountRepository
    {
        Task<IEnumerable<UserAccountModel>> GetUserAccountPg(int pageNum, int pageItem);
        Task<UserAccountPagedMdl?> GetUserAccPaged(int pageNum, int pageItem);
        Task<IEnumerable<UserAccountModel>> GetActiveUserAccountPg(int pageNum, int pageItem);
        Task<IEnumerable<UserAccountModel>> GetInActiveUserAccountPg(int pageNum, int pageItem);
        Task<IEnumerable<UserAccountModel>> GetUserAccountPgSrch(string searchKey, int pageNum, int pageItem);
        Task<UserAccountPagedMdl?> GetUserAccSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<UserAccountModel> GetUserAccountById(string userAccountId);
        Task<UserAccountModel> GetUserAccWithPassById(string userAccountId);
        Task<IEnumerable<UserAccountModel>> GetUserAccountPgFiltered(UserAccountFilterMdl filter, int pageNum, int pageItem);
        Task<UserAccountPagedMdl?> GetUserAccFltrPaged(UserAccountFilterMdl filter, int pageNum, int pageItem);
        Task<bool> UserAccountExists(string userAccountId);
        Task<IEnumerable<UserAccountModel>> GetUsrAccByAccessRightIdPg(string accessRightId, int pageNum, int pageItem);
        Task<CreateUserAccResultCode> CreateUserAccount(UserAccountModelMod userAccount);
        Task<bool> UpdateUserAccount(UserAccountModelMod userAccount);
        Task<bool> DeleteUserAccount(string delUserAccountId, string userAccountId);
        Task<bool> DeActUserAccount(string delUserAccountId, string userAccountId);
        Task<bool> ReActUserAccount(string delUserAccountId, string userAccountId);
        Task<AccValidationResult> UserAccountValidationWeb(LoginCredentailsModel account);
        Task<AccValidationResult> UserAccountValidationMob(LoginCredentailsModel account);
    }
}
