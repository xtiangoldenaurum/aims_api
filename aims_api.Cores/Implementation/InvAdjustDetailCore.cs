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
    public class InvAdjustDetailCore : IInvAdjustDetailCore
    {
        private IInvAdjustDetailRepository InvAdjustDetailRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvAdjustDetailCore(IInvAdjustDetailRepository invAdjustDetailRepo, EnumHelper enumHelper)
        {
            InvAdjustDetailRepo = invAdjustDetailRepo;
            EnumHelper = enumHelper;
        }
        public async Task<RequestResponse> CreateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail)
        {
            bool invAdjustDetailExists = await InvAdjustDetailRepo.InvAdjustDetailExists(invAdjustDetail.InvAdjustLineId);
            if (invAdjustDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvAdjustLineId exists.");
            }

            bool res = await InvAdjustDetailRepo.CreateInvAdjustDetail(invAdjustDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> DeleteInvAdjustDetail(string invAdjustLineId)
        {
            // place item in use validator here

            bool res = await InvAdjustDetailRepo.DeleteInvAdjustDetail(invAdjustLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeleteInvAdjustDetailMod(string invAdjustLineId, string userAccountId)
        {
            var res = await InvAdjustDetailRepo.DeleteInvAdjustDetailMod(invAdjustLineId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvAdjustDetailDelResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> GetInvAdjustDetailById(string invAdjustLineId)
        {
            var data = await InvAdjustDetailRepo.GetInvAdjustDetailById(invAdjustLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustDetailByInvAdjustIDPaged(string invAdjustId, int pageNum, int pageItem)
        {
            var data = await InvAdjustDetailRepo.GetInvAdjustDetailByInvAdjustIDPaged(invAdjustId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustDetailByInvAdjustIDPagedMod(string invAdjustId, int pageNum, int pageItem)
        {
            var data = await InvAdjustDetailRepo.GetInvAdjustDetailByInvAdjustIDPagedMod(invAdjustId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustDetailPg(int pageNum, int pageItem)
        {
            var data = await InvAdjustDetailRepo.GetInvAdjustDetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvAdjustDetailRepo.GetInvAdjustDetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> UpdateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail)
        {
            bool res = await InvAdjustDetailRepo.UpdateInvAdjustDetail(invAdjustDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }
    }
}
