﻿using aims_api.Cores.Interface;
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
    public class InvMoveUserFieldCore : IInvMoveUserFieldCore
    {
        private IInvMoveUserFieldRepository InvMoveUserFieldRepo { get; set; }
        public InvMoveUserFieldCore(IInvMoveUserFieldRepository invMoveUserFieldRepo)
        {
            InvMoveUserFieldRepo = invMoveUserFieldRepo;
        }
        public async Task<RequestResponse> CreateInvMoveUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            bool columnExists = await InvMoveUserFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            bool res = await InvMoveUserFieldRepo.CreateInvMoveUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> DeleteInvMoveUField(string fieldName, string userAccountId)
        {
            // place item in use validator here

            bool res = await InvMoveUserFieldRepo.DeleteInvMoveUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> GetInvMoveUFields()
        {
            var data = await InvMoveUserFieldRepo.GetInvMoveUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveUserFieldById(string invMoveId)
        {
            var data = await InvMoveUserFieldRepo.GetInvMoveUserFieldById(invMoveId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveUserFieldPg(int pageNum, int pageItem)
        {
            var data = await InvMoveUserFieldRepo.GetInvMoveUserFieldPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveUserFieldPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvMoveUserFieldRepo.GetInvMoveUserFieldPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> UpdateInvMoveUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            bool res = await InvMoveUserFieldRepo.UpdateInvMoveUField(oldFieldName, newFieldName, modifiedBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }
    }
}
