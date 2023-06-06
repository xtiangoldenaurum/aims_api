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
    public class ReturnsUserFieldCore : IReturnsUserFieldCore
    {
        private IReturnsUserFieldRepository ReturnsUserFieldRepo { get; set; }
        public ReturnsUserFieldCore(IReturnsUserFieldRepository returnsUserFieldRepo)
        {
            ReturnsUserFieldRepo = returnsUserFieldRepo;
        }

        public async Task<RequestResponse> GetReturnsUserFieldPg(int pageNum, int pageItem)
        {   
            var data = await ReturnsUserFieldRepo.GetReturnsUserFieldPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsUserFieldPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ReturnsUserFieldRepo.GetReturnsUserFieldPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsUserFieldById(string returnsId)
        {
            var data = await ReturnsUserFieldRepo.GetReturnsUserFieldById(returnsId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsUFields()
        {
            var data = await ReturnsUserFieldRepo.GetReturnsUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateReturnsUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await ReturnsUserFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await ReturnsUserFieldRepo.CreateReturnsUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateReturnsUField(string oldFieldName, string newFieldName, string modifiedByd)
        {
            bool res = await ReturnsUserFieldRepo.UpdateReturnsUField(oldFieldName, newFieldName, modifiedByd);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteReturnsUField(string fieldName, string userAccountId)
        {
            // place item in use validator here

            bool res = await ReturnsUserFieldRepo.DeleteReturnsUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

    }
}
