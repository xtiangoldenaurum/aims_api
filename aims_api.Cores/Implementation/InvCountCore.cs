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
    public class InvCountCore : IInvCountCore
    {

        private IInvCountRepository InvCountRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvCountCore(IInvCountRepository invCountRepo, EnumHelper enumHelper)
        {
            InvCountRepo = invCountRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetInvCountSpecial(InvCountFilteredMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            InvCountPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.InvCountId) ||
                !string.IsNullOrEmpty(filter.InvCountStatusId) ||
                filter.DateCreated != null)
            {
                data = await InvCountRepo.GetInvCountFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await InvCountRepo.GetInvCountSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await InvCountRepo.GetInvCountPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountForCntPaged(int pageNum, int pageItem)
        {
            var data = await InvCountRepo.GetInvCountForCntPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountPg(int pageNum, int pageItem)
        {
            var data = await InvCountRepo.GetInvCountPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvCountRepo.GetInvCountPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountById(string invCountId)
        {
            var data = await InvCountRepo.GetInvCountById(invCountId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountByIdMod(string invCountId)
        {
            var invCountHeader = await InvCountRepo.GetInvCountById(invCountId);
            //var userFields = await InvCountUFieldRepo.GetInvMoveUserFieldById(invCountId);

            if (invCountHeader != null)
            {
                var data = new InvCountModelMod()
                {
                    InvCountHeader = invCountHeader,
                    //InvCountUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvCountMod(InvCountModelMod invCount)
        {
            var res = await InvCountRepo.CreateInvCountMod(invCount);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == InvCountTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.InvCountId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());

        }

        public async Task<RequestResponse> UpdateInvCountMod(InvCountModelMod invCount)
        {
            var res = await InvCountRepo.UpdateInvCountMod(invCount);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvCountTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> DeleteInvCount(string invCountId)
        {
            // place item in use validator here

            bool res = await InvCountRepo.DeleteInvCount(invCountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> CancelInvCount(string invCountId, string userAccountId)
        {
            var res = await InvCountRepo.CancelInvCount(invCountId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelInvCountResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        #region ForceCancelInvMove
        //public async Task<RequestResponse> ForceCancelInvMove(string invMoveId, string userAccountId)
        //{
        //    var res = await InvMoveRepo.ForceCancelInvMove(invMoveId, userAccountId);
        //    string resMsg = await EnumHelper.GetDescription(res);

        //    if (res == CancelInvMoveResultCode.SUCCESS)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        //}
        #endregion
    }
}
