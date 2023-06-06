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
    public class SOUserFieldCore : ISOUserFieldCore
    {
        private ISOUserFieldRepository SOUserFieldRepo { get; set; }
        public SOUserFieldCore(ISOUserFieldRepository soUserFieldRepo)
        {
            SOUserFieldRepo = soUserFieldRepo;
        }

        public async Task<RequestResponse> GetSOUserFieldById(string soId)
        {
            var data = await SOUserFieldRepo.GetSOUserFieldById(soId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOUFields()
        {
            var data = await SOUserFieldRepo.GetSOUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateSOUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await SOUserFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await SOUserFieldRepo.CreateSOUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateSOUField(string oldFieldName, string newFieldName, string modifiedByd)
        {
            bool res = await SOUserFieldRepo.UpdateSOUField(oldFieldName, newFieldName, modifiedByd);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteSOUField(string fieldName, string userAccountId)
        {
            // place item in use validator here

            bool res = await SOUserFieldRepo.DeleteSOUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
