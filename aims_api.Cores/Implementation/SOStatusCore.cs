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
    public class SOStatusCore : ISOStatusCore
    {
        private ISOStatusRepository SOStatusRepo { get; set; }
        public SOStatusCore(ISOStatusRepository soStatusRepo)
        {
            SOStatusRepo = soStatusRepo;
        }

        public async Task<RequestResponse> GetSOStatusPg(int pageNum, int pageItem)
        {   
            var data = await SOStatusRepo.GetSOStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await SOStatusRepo.GetSOStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetSOStatusById(string soStatusId)
        {
            var data = await SOStatusRepo.GetSOStatusById(soStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateSOStatus(SOStatusModel soStatus)
        {
            bool soStatusExists = await SOStatusRepo.SOStatusExists(soStatus.SoStatusId);
            if (soStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar SoStatusId exists.");
            }

            bool res = await SOStatusRepo.CreateSOStatus(soStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateSOStatus(SOStatusModel soStatus)
        {
            bool res = await SOStatusRepo.UpdateSOStatus(soStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteSOStatus(string soStatusId)
        {
			// place item in use validator here

            bool res = await SOStatusRepo.DeleteSOStatus(soStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
