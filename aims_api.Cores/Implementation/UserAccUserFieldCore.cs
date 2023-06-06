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
    public class UserAccUserFieldCore : IUserAccUserFieldCore
    {
        private IUsrAccUserFieldRepository UserAccUFieldRepo { get; set; }
        public UserAccUserFieldCore(IUsrAccUserFieldRepository userAccUFieldRepo)
        {
            UserAccUFieldRepo = userAccUFieldRepo;
        }

        public async Task<RequestResponse> GetAccUserFieldById(string userAccountId)
        {
            var data = await UserAccUFieldRepo.GetAccUserFieldById(userAccountId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetUserAccountUFields()
        {
            var data = await UserAccUFieldRepo.GetUserAccountUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateUserAccUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await UserAccUFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await UserAccUFieldRepo.CreateUserAccUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateUserAccUField(string oldFieldName, string newFieldName, string modifiedByd)
        {
            bool res = await UserAccUFieldRepo.UpdateUserAccUField(oldFieldName, newFieldName, modifiedByd);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteUserAccUField(string fieldName, string userAccountId)
        {
            // place item in use validator here

            bool res = await UserAccUFieldRepo.DeleteUserAccUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
