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
    public class IdNumberCore : IIdNumberCore
    {
        private IIdNumberRepository IdNumberRepo { get; set; }
        public IdNumberCore(IIdNumberRepository idNumberRepo)
        {
            IdNumberRepo = idNumberRepo;
        }

        public async Task<RequestResponse> GetIdNumberPg(int pageNum, int pageItem)
        {   
            var data = await IdNumberRepo.GetIdNumberPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetIdNumberPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await IdNumberRepo.GetIdNumberPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetIdNumberById(string transactionTypeId)
        {
            var data = await IdNumberRepo.GetIdNumberById(transactionTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetNextIdNum(string tranTypeId)
        {
            var data = await IdNumberRepo.GetNextIdNum(tranTypeId);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetNxtDocNum(string tranTypeId, string userAccountId)
        {
            var data = await IdNumberRepo.GetNxtDocNum(tranTypeId, userAccountId);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateIdNumber(IdNumberModel idNumber)
        {
            bool idNumberExists = await IdNumberRepo.IdNumberExists(idNumber.TransactionTypeId);
            if (idNumberExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar TransactionTypeId exists.");
            }

            bool res = await IdNumberRepo.CreateIdNumber(idNumber);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateIdNumber(IdNumberModel idNumber)
        {
            // check id length not exceeding 12 character
            var len = 0;

            if (!string.IsNullOrEmpty(idNumber.Prefix))
            {
                len += idNumber.Prefix.Length;
            }

            if (!string.IsNullOrEmpty(idNumber.Suffix))
            {
                len += idNumber.Suffix.Length;
            }

            if (!string.IsNullOrEmpty(idNumber.ZeroesLength))
            {
                len += idNumber.ZeroesLength.Length;
            }

            if (len > 12)
            {
                return new RequestResponse(ResponseCode.FAILED, "Provide data (prefix + zero length + suffix) not exceeding 12 character.");
            }

            bool res = await IdNumberRepo.UpdateIdNumber(idNumber);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteIdNumber(string transactionTypeId, string userAccountId)
        {
			// place item in use validator here

            bool res = await IdNumberRepo.DeleteIdNumber(transactionTypeId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> PrePrintEPC(int count)
        {
            var data = await IdNumberRepo.PrePrintEPC(count);

            if (data != null)
            {
                if (data.LabelsToPrint != null && data.ZplDetails != null)
                {
                    return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
                }
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> PrePrintLPN(int count)
        {
            var data = await IdNumberRepo.PrePrintLPN(count);

            if (data != null)
            {
                if (data.LabelsToPrint != null && data.ZplDetails != null)
                {
                    return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
                }
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
