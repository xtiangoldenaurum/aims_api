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
    public class WhTransUserFieldCore : IWhTransUserFieldCore
    {
        private IWhTransUserFieldRepository WhTransUserFieldRepo { get; set; }
        public WhTransUserFieldCore(IWhTransUserFieldRepository whTransUserFieldRepo)
        {
            WhTransUserFieldRepo = whTransUserFieldRepo;
        }

        public async Task<RequestResponse> GetWhTransUserFieldById(string whTransferId)
        {
            var data = await WhTransUserFieldRepo.GetWhTransUserFieldById(whTransferId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferUFields()
        {
            var data = await WhTransUserFieldRepo.GetWhTransferUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateWhTransferUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await WhTransUserFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await WhTransUserFieldRepo.CreateWhTransferUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateWhTransferUField(string oldFieldName, string newFieldName, string modifiedByd)
        {
            bool res = await WhTransUserFieldRepo.UpdateWhTransferUField(oldFieldName, newFieldName, modifiedByd);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteWhTransferUField(string fieldName, string userAccountId)
        {
            // place item in use validator here

            bool res = await WhTransUserFieldRepo.DeleteWhTransferUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
