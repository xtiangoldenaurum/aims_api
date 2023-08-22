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
    public class InvCountDetailCore : IInvCountDetailCore
    {
        private IInvCountDetailRepository InvCountDetailRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public InvCountDetailCore(IInvCountDetailRepository invCountDetailRepo, EnumHelper enumHelper)
        {
            InvCountDetailRepo = invCountDetailRepo;
            EnumHelper = enumHelper;
        }

        #region GetInvCountDetailByInvCountIDPaged
        //public async Task<RequestResponse> GetInvCountDetailByInvCountIDPaged(string invCountId, int pageNum, int pageItem)
        //{
        //    var data = await InvCountDetailRepo.GetInvCountDetailByInvCountIDPaged(invCountId, pageNum, pageItem);

        //    if (data != null && data.Pagination != null)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "No record found.");
        //}
        #endregion

        public async Task<RequestResponse> GetInvCountDetailByInvCountIDPagedMod(string invCountId, int pageNum, int pageItem)
        {
            var data = await InvCountDetailRepo.GetInvCountDetailByInvCountIDPagedMod(invCountId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountDetailPg(int pageNum, int pageItem)
        {
            var data = await InvCountDetailRepo.GetInvCountDetailPg(pageNum, pageItem);
                
            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvCountDetailRepo.GetInvCountDetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountDetailById(string invCountLineId)
        {
            var data = await InvCountDetailRepo.GetInvCountDetailById(invCountLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvCountDetail(InvCountDetailModel invCountDetail)
        {
            bool invCountDetailExists = await InvCountDetailRepo.InvCountDetailExists(invCountDetail.InvCountLineId);
            if (invCountDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvCountLineId exists.");
            }

            bool res = await InvCountDetailRepo.CreateInvCountDetail(invCountDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvCountDetail(InvCountDetailModel invCountDetail)
        {
            bool res = await InvCountDetailRepo.UpdateInvCountDetail(invCountDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvCountDetail(string invCountLineId)
        {
            // place item in use validator here

            bool res = await InvCountDetailRepo.DeleteInvCountDetail(invCountLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeleteInvCountDetailMod(string invCountLineId, string userAccountId)
        {
            var res = await InvCountDetailRepo.DeleteInvCountDetailMod(invCountLineId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == InvCountDetailDelResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }
    }
}
