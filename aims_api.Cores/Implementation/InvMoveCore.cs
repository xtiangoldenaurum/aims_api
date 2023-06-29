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
    public class InvMoveCore : IInvMoveCore
    {
        private IInvMoveRepository InvMoveRepo { get; set; }
        private IInvMoveUserFieldRepository InvMoveUFieldRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvMoveCore(IInvMoveRepository invMoveRepo, IInvMoveUserFieldRepository invMoveUserFieldRepo, EnumHelper enumHelper)
        {
            InvMoveRepo = invMoveRepo;
            InvMoveUFieldRepo = invMoveUserFieldRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetInvMoveSpecial(InvMoveFilteredMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            InvMovePagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.InvMoveId) ||
                !string.IsNullOrEmpty(filter.InvMoveStatusId) ||
                filter.DateCreated != null)
            {
                data = await InvMoveRepo.GetInvMoveFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await InvMoveRepo.GetInvMoveSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await InvMoveRepo.GetInvMovePaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveForMvPaged(int pageNum, int pageItem)
        {
            var data = await InvMoveRepo.GetInvMoveForMvPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMovePg(int pageNum, int pageItem)
        {
            var data = await InvMoveRepo.GetInvMovePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMovePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvMoveRepo.GetInvMovePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveById(string invMoveId)
        {
            var data = await InvMoveRepo.GetInvMoveById(invMoveId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveByIdMod(string invMoveId)
        {
            var invMoveHeader = await InvMoveRepo.GetInvMoveById(invMoveId);
            var userFields = await InvMoveUFieldRepo.GetInvMoveUserFieldById(invMoveId);

            if (invMoveHeader != null)
            {
                var data = new InvMoveModelMod()
                {
                    InvMoveHeader = invMoveHeader,
                    InvMoveUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
        public async Task<RequestResponse> CreateInvMoveMod(InvMoveModelMod invMove)
        {
            var res = await InvMoveRepo.CreateInvMoveMod(invMove);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == InvMoveTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.InvMoveId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());

        }
        public async Task<RequestResponse> UpdateInvMoveMod(InvMoveModelMod invMove)
        {
            var res = await InvMoveRepo.UpdateInvMoveMod(invMove);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvMoveTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> DeleteInvMove(string invMoveId)
        {
            // place item in use validator here

            bool res = await InvMoveRepo.DeleteInvMove(invMoveId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> CancelInvMove(string invMoveId, string userAccountId)
        {
            var res = await InvMoveRepo.CancelInvMove(invMoveId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelInvMoveResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> ForceCancelInvMove(string invMoveId, string userAccountId)
        {
            var res = await InvMoveRepo.ForceCancelInvMove(invMoveId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelInvMoveResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }
    }
}
