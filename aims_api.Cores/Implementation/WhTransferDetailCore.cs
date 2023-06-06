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
    public class WhTransferDetailCore : IWhTransferDetailCore
    {
        private IWhTransferDetailRepository WhTransferDetailRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public WhTransferDetailCore(IWhTransferDetailRepository whTransferDetailRepo, EnumHelper enumHelper)
        {
            WhTransferDetailRepo = whTransferDetailRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetWhTransDetailByTrnasIdPagedMod(string whTransId, int pageNum, int pageItem)
        {
            var data = await WhTransferDetailRepo.GetWhTransDetailByTrnasIdPagedMod(whTransId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferDetailPg(int pageNum, int pageItem)
        {   
            var data = await WhTransferDetailRepo.GetWhTransferDetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await WhTransferDetailRepo.GetWhTransferDetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferDetailById(string whTransferLineId)
        {
            var data = await WhTransferDetailRepo.GetWhTransferDetailById(whTransferLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateWhTransferDetail(WhTransferDetailModel whTransferDetail)
        {
            bool whTransferDetailExists = await WhTransferDetailRepo.WhTransferDetailExists(whTransferDetail.WhTransferLineId);
            if (whTransferDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar WhTransferLineId exists.");
            }

            bool res = await WhTransferDetailRepo.CreateWhTransferDetail(whTransferDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateWhTransferDetail(WhTransferDetailModel whTransferDetail)
        {
            bool res = await WhTransferDetailRepo.UpdateWhTransferDetail(whTransferDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteWhTransferDetail(string whTransferLineId)
        {
			// place item in use validator here

            bool res = await WhTransferDetailRepo.DeleteWhTransferDetail(whTransferLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeleteWhTransferDetailMmod(string whTransferLineId, string userAccountId)
        {
            var res = await WhTransferDetailRepo.DeleteWhTransDetailMod(whTransferLineId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == WhTransDetailDelResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

    }
}
