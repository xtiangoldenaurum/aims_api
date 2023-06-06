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
    public class PODetailCore : IPODetailCore
    {
        private IPODetailRepository PODetailRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }

        public PODetailCore(IPODetailRepository poDetailRepo, EnumHelper enumHelper)
        {
            PODetailRepo = poDetailRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetPODetailByPoIDPaged(string poId, int pageNum, int pageItem)
        {
            var data = await PODetailRepo.GetPODetailByPoIDPaged(poId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPODetailByPoIDPagedMod(string poId, int pageNum, int pageItem)
        {
            var data = await PODetailRepo.GetPODetailByPoIDPagedMod(poId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPODetailPg(int pageNum, int pageItem)
        {   
            var data = await PODetailRepo.GetPODetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPODetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await PODetailRepo.GetPODetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPODetailById(string poLineId)
        {
            var data = await PODetailRepo.GetPODetailById(poLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePODetail(PODetailModel poDetail)
        {
            bool poDetailExists = await PODetailRepo.PODetailExists(poDetail.PoLineId);
            if (poDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar PoLineId exists.");
            }

            bool res = await PODetailRepo.CreatePODetail(poDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdatePODetail(PODetailModel poDetail)
        {
            bool res = await PODetailRepo.UpdatePODetail(poDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeletePODetail(string poLineId)
        {
			// place item in use validator here

            bool res = await PODetailRepo.DeletePODetail(poLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> DeletePODetailMod(string poLineId, string userAccountId)
        {
            var res = await PODetailRepo.DeletePODetailMod(poLineId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == PODetailDelResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }
    }
}
