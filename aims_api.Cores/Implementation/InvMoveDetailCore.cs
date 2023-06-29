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
    public class InvMoveDetailCore : IInvMoveDetailCore
    {
        private IInvMoveDetailRepository InvMoveDetailRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvMoveDetailCore(IInvMoveDetailRepository invMoveDetailRepo, EnumHelper enumHelper)
        {
            InvMoveDetailRepo = invMoveDetailRepo;
            EnumHelper = enumHelper;
        }
        public async Task<RequestResponse> CreateInvMoveDetail(InvMoveDetailModel invMoveDetail)
        {
            bool invMoveDetailExists = await InvMoveDetailRepo.InvMoveDetailExists(invMoveDetail.InvMoveLineId);
            if (invMoveDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvMoveLineId exists.");
            }

            bool res = await InvMoveDetailRepo.CreateInvMoveDetail(invMoveDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> DeleteInvMoveDetail(string invMoveLineId)
        {
            // place item in use validator here

            bool res = await InvMoveDetailRepo.DeleteInvMoveDetail(invMoveLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeleteInvMoveDetailMod(string invMoveLineId, string userAccountId)
        {
            var res = await InvMoveDetailRepo.DeleteInvMoveDetailMod(invMoveLineId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvMoveDetailDelResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> GetInvMoveDetailById(string invMoveLineId)
        {
            var data = await InvMoveDetailRepo.GetInvMoveDetailById(invMoveLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveDetailByInvMoveIDPaged(string invMoveId, int pageNum, int pageItem)
        {
            var data = await InvMoveDetailRepo.GetInvMoveDetailByInvMoveIDPaged(invMoveId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveDetailByInvMoveIDPagedMod(string invMoveId, int pageNum, int pageItem)
        {
            var data = await InvMoveDetailRepo.GetInvMoveDetailByInvMoveIDPagedMod(invMoveId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveDetailPg(int pageNum, int pageItem)
        {
            var data = await InvMoveDetailRepo.GetInvMoveDetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvMoveDetailRepo.GetInvMoveDetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> UpdateInvMoveDetail(InvMoveDetailModel invMoveDetail)
        {
            bool res = await InvMoveDetailRepo.UpdateInvMoveDetail(invMoveDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }
    }
}
