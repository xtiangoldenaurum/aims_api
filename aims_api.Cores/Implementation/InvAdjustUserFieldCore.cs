using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class InvAdjustUserFieldCore : IInvAdjustUserFieldCore
    {
        private IInvAdjustUserFieldRepository InvAdjustUserFieldRepo { get; set; }
        public InvAdjustUserFieldCore(IInvAdjustUserFieldRepository invAdjustUserFieldRepo)
        {
            InvAdjustUserFieldRepo = invAdjustUserFieldRepo;
        }
        public async Task<RequestResponse> CreateInvAdjustUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await InvAdjustUserFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await InvAdjustUserFieldRepo.CreateInvAdjustUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> DeleteInvAdjustUField(string fieldName, string userAccountId)
        {
            // place item in use validator here

            bool res = await InvAdjustUserFieldRepo.DeleteInvAdjustUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> GetInvAdjustUFields()
        {
            var data = await InvAdjustUserFieldRepo.GetInvAdjustUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustUserFieldById(string invAdjustId)
        {
            var data = await InvAdjustUserFieldRepo.GetInvAdjustUserFieldById(invAdjustId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustUserFieldPg(int pageNum, int pageItem)
        {
            var data = await InvAdjustUserFieldRepo.GetInvAdjustUserFieldPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustUserFieldPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvAdjustUserFieldRepo.GetInvAdjustUserFieldPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> UpdateInvAdjustUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            bool res = await InvAdjustUserFieldRepo.UpdateInvAdjustUField(oldFieldName, newFieldName, modifiedBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }
    }
}
