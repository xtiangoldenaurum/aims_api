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
    public class InvAdjustCore : IInvAdjustCore
    {
        private IInvAdjustRepository InvAdjustRepo { get; set; }
        private IInvAdjustUserFieldRepository InvAdjustUFieldRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvAdjustCore(IInvAdjustRepository invAdjustRepo, IInvAdjustUserFieldRepository invAdjustUserFieldRepo, EnumHelper enumHelper)
        {
            InvAdjustRepo = invAdjustRepo;
            InvAdjustUFieldRepo = invAdjustUserFieldRepo;
            EnumHelper = enumHelper;
        }
        public async Task<RequestResponse> CancelInvAdjust(string invAdjustId, string userAccountId)
        {
            var res = await InvAdjustRepo.CancelInvAdjust(invAdjustId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelInvAdjustResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> CreateInvAdjustMod(InvAdjustModelMod invAdjust)
        {
            var res = await InvAdjustRepo.CreateInvAdjustMod(invAdjust);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == InvAdjustTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.InvAdjustId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());
        }

        public async Task<RequestResponse> DeleteInvAdjust(string invAdjustId)
        {
            // place item in use validator here

            bool res = await InvAdjustRepo.DeleteInvAdjust(invAdjustId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> ForceCancelInvAdjust(string invAdjustId, string userAccountId)
        {
            var res = await InvAdjustRepo.ForceCancelInvAdjust(invAdjustId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelInvAdjustResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> GetInvAdjustById(string invAdjustId)
        {
            var data = await InvAdjustRepo.GetInvAdjustById(invAdjustId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustByIdMod(string invAdjustId)
        {
            var invAdjustHeader = await InvAdjustRepo.GetInvAdjustById(invAdjustId);
            var userFields = await InvAdjustUFieldRepo.GetInvAdjustUserFieldById(invAdjustId);

            if (invAdjustHeader != null)
            {
                var data = new InvAdjustModelMod()
                {
                    InvAdjustHeader = invAdjustHeader,
                    InvAdjustUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustForAdjPaged(int pageNum, int pageItem)
        {
            var data = await InvAdjustRepo.GetInvAdjustForAdjPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustPg(int pageNum, int pageItem)
        {
            var data = await InvAdjustRepo.GetInvAdjustPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvAdjustRepo.GetInvAdjustPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustSpecial(InvAdjustFilteredMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            InvAdjustPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.InvAdjustId) ||
                !string.IsNullOrEmpty(filter.InvAdjustStatusId) ||
                filter.DateCreated != null)
            {
                data = await InvAdjustRepo.GetInvAdjustFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await InvAdjustRepo.GetInvAdjustSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await InvAdjustRepo.GetInvAdjustPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> UpdateInvAdjustMod(InvAdjustModelMod invAdjust)
        {
            var res = await InvAdjustRepo.UpdateInvAdjustMod(invAdjust);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvAdjustTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> UpdateInvAdjustApprovedMod(InvAdjustModelMod invAdjust)
        {
            var res = await InvAdjustRepo.UpdateInvAdjustMod(invAdjust);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvAdjustTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }
    }
}
