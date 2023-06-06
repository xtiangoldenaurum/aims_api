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
    public class LotAttributeDetailCore : ILotAttributeDetailCore
    {
        private ILotAttributeDetailRepository LotAttributeDetailRepo { get; set; }
        public LotAttributeDetailCore(ILotAttributeDetailRepository lotAttributeDetailRepo)
        {
            LotAttributeDetailRepo = lotAttributeDetailRepo;
        }

        public async Task<RequestResponse> GetLotAttributeDetailPg(int pageNum, int pageItem)
        {   
            var data = await LotAttributeDetailRepo.GetLotAttributeDetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLotAttributeDetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await LotAttributeDetailRepo.GetLotAttributeDetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetLotAttributeDetailById(string lotAttributeId)
        {
            var data = await LotAttributeDetailRepo.GetLotAttributeDetailById(lotAttributeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail)
        {
            bool lotAttributeDetailExists = await LotAttributeDetailRepo.LotAttributeDetailExists(lotAttributeDetail.LotAttributeId);
            if (lotAttributeDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar LotAttributeId exists.");
            }

            bool res = await LotAttributeDetailRepo.CreateLotAttributeDetail(lotAttributeDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail)
        {
            bool res = await LotAttributeDetailRepo.UpdateLotAttributeDetail(lotAttributeDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteLotAttributeDetail(string lotAttributeId, string userAccountId)
        {
			// place item in use validator here

            bool res = await LotAttributeDetailRepo.DeleteLotAttributeDetail(lotAttributeId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
