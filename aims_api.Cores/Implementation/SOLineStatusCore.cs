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
    public class SOLineStatusCore : ISOLineStatusCore
    {
        private ISOLineStatusRepository SOLineStatusRepo { get; set; }
        public SOLineStatusCore(ISOLineStatusRepository soLineStatusRepo)
        {
            SOLineStatusRepo = soLineStatusRepo;
        }

        public async Task<RequestResponse> GetSOLineStatusPg(int pageNum, int pageItem)
        {   
            var data = await SOLineStatusRepo.GetSOLineStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await SOLineStatusRepo.GetSOLineStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOLineStatusById(string soLineStatusId)
        {
            var data = await SOLineStatusRepo.GetSOLineStatusById(soLineStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateSOLineStatus(SOLineStatusModel soLineStatus)
        {
            bool soLineStatusExists = await SOLineStatusRepo.SOLineStatusExists(soLineStatus.SoLineStatusId);
            if (soLineStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar SoLineStatusId exists.");
            }

            bool res = await SOLineStatusRepo.CreateSOLineStatus(soLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateSOLineStatus(SOLineStatusModel soLineStatus)
        {
            bool res = await SOLineStatusRepo.UpdateSOLineStatus(soLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteSOLineStatus(string soLineStatusId)
        {
			// place item in use validator here

            bool res = await SOLineStatusRepo.DeleteSOLineStatus(soLineStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
