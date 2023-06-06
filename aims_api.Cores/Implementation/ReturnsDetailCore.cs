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
    public class ReturnsDetailCore : IReturnsDetailCore
    {
        private IReturnsDetailRepository ReturnsDetailRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public ReturnsDetailCore(IReturnsDetailRepository returnsDetailRepo, EnumHelper enumHelper)
        {
            ReturnsDetailRepo = returnsDetailRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetRetDetailByRetIdPagedMod(string returnsId, int pageNum, int pageItem)
        {
            var data = await ReturnsDetailRepo.GetRetDetailByRetIdPagedMod(returnsId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsDetailPg(int pageNum, int pageItem)
        {   
            var data = await ReturnsDetailRepo.GetReturnsDetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ReturnsDetailRepo.GetReturnsDetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsDetailById(string returnsLineId)
        {
            var data = await ReturnsDetailRepo.GetReturnsDetailById(returnsLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateReturnsDetail(ReturnsDetailModel returnsDetail)
        {
            bool returnsDetailExists = await ReturnsDetailRepo.ReturnsDetailExists(returnsDetail.ReturnsLineId);
            if (returnsDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ReturnsLineId exists.");
            }

            bool res = await ReturnsDetailRepo.CreateReturnsDetail(returnsDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateReturnsDetail(ReturnsDetailModel returnsDetail)
        {
            bool res = await ReturnsDetailRepo.UpdateReturnsDetail(returnsDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteReturnsDetail(string returnsLineId)
        {
			// place item in use validator here

            bool res = await ReturnsDetailRepo.DeleteReturnsDetail(returnsLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeleteReturnsDetailMod(string returnsLineId, string userAccountId)
        {
            var res = await ReturnsDetailRepo.DeleteRetDetailMod(returnsLineId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == RetDetailDelResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

    }
}
