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
    public class POUserFieldCore : IPOUserFieldCore
    {
        private IPOUserFieldRepository POUserFieldRepo { get; set; }
        public POUserFieldCore(IPOUserFieldRepository poUserFieldRepo)
        {
            POUserFieldRepo = poUserFieldRepo;
        }

        public async Task<RequestResponse> GetPOUserFieldPg(int pageNum, int pageItem)
        {   
            var data = await POUserFieldRepo.GetPOUserFieldPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOUserFieldPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await POUserFieldRepo.GetPOUserFieldPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOUserFieldById(string poId)
        {
            var data = await POUserFieldRepo.GetPOUserFieldById(poId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOUFields()
        {
            var data = await POUserFieldRepo.GetPOUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePOUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await POUserFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await POUserFieldRepo.CreatePOUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdatePOUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            bool res = await POUserFieldRepo.UpdatePOUField(oldFieldName, newFieldName, modifiedBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeletePOUField(string fieldName, string userAccountId)
        {
			// place item in use validator here

            bool res = await POUserFieldRepo.DeletePOUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
