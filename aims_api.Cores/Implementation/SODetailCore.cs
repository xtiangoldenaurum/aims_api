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
    public class SODetailCore : ISODetailCore
    {
        private ISODetailRepository SODetailRepo { get; set; }
        public SODetailCore(ISODetailRepository soDetailRepo)
        {
            SODetailRepo = soDetailRepo;
        }

        public async Task<RequestResponse> GetSODetailPg(int pageNum, int pageItem)
        {   
            var data = await SODetailRepo.GetSODetailPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSODetailPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await SODetailRepo.GetSODetailPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSODetailById(string soLineId)
        {
            var data = await SODetailRepo.GetSODetailById(soLineId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateSODetail(SODetailModel soDetail)
        {
            bool soDetailExists = await SODetailRepo.SODetailExists(soDetail.SoLineId);
            if (soDetailExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar SoLineId exists.");
            }

            bool res = await SODetailRepo.CreateSODetail(soDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateSODetail(SODetailModel soDetail)
        {
            bool res = await SODetailRepo.UpdateSODetail(soDetail);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteSODetail(string soLineId)
        {
			// place item in use validator here

            bool res = await SODetailRepo.DeleteSODetail(soLineId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
