using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class UserAccountCore : IUserAccountCore
    {
        private IUserAccountRepository UserAccountRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public UserAccountCore(IUserAccountRepository userAccountRepo, EnumHelper enumHelper)
        {
            UserAccountRepo = userAccountRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetUserAccountsSpecial(UserAccountFilterMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            UserAccountPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.AccessRightId) ||
                filter.Inactive != null ||
                filter.AccountExpiry != null)
            {
                data = await UserAccountRepo.GetUserAccFltrPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await UserAccountRepo.GetUserAccSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await UserAccountRepo.GetUserAccPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccountPg(int pageNum, int pageItem)
        {   
            var data = await UserAccountRepo.GetUserAccountPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccPaged(int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetUserAccPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetActiveUserAccountPg(int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetActiveUserAccountPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInActiveUserAccountPg(int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetInActiveUserAccountPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccountPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetUserAccountPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetUserAccSrchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccountById(string userAccountId)
        {
            var data = await UserAccountRepo.GetUserAccountById(userAccountId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccountPgFiltered(UserAccountFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetUserAccountPgFiltered(filter, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccFltrPaged(UserAccountFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetUserAccFltrPaged(filter, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUsrAccByAccessRightIdPg(string accessRightId, int pageNum, int pageItem)
        {
            var data = await UserAccountRepo.GetUsrAccByAccessRightIdPg(accessRightId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateUserAccount(UserAccountModelMod userAccount)
        {
            var res = await UserAccountRepo.CreateUserAccount(userAccount);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CreateUserAccResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> UpdateUserAccount(UserAccountModelMod userAccount)
        {
            bool res = await UserAccountRepo.UpdateUserAccount(userAccount);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteUserAccount(string delUserAccountId, string userAccountId)
        {
			// place item in use validator here

            bool res = await UserAccountRepo.DeleteUserAccount(delUserAccountId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeActUserAccount(string delUserAccountId, string userAccountId)
        {
            // place item in use validator here

            bool res = await UserAccountRepo.DeActUserAccount(delUserAccountId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> ReActUserAccount(string delUserAccountId, string userAccountId)
        {
            // place item in use validator here

            bool res = await UserAccountRepo.ReActUserAccount(delUserAccountId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> UserAccountValidationWeb(LoginCredentailsModel account)
        {
            var data = await UserAccountRepo.UserAccountValidationWeb(account);
            string resMsg = await EnumHelper.GetDescription(data.ResultCode);

            if (data != null && 
                data.UserAccount != null && 
                data.WebUserAccessRights != null &&
                data.WebUserAccessRights.Any() && 
                data.ResultCode == LoginResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, data);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }

        public async Task<RequestResponse> UserAccountValidationMob(LoginCredentailsModel account)
        {
            var data = await UserAccountRepo.UserAccountValidationMob(account);
            string resMsg = await EnumHelper.GetDescription(data.ResultCode);

            if (data != null &&
                data.UserAccount != null &&
                data.MobUserAccessRights != null &&
                data.MobUserAccessRights.Any() &&
                data.ResultCode == LoginResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, data);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }
    }
}
