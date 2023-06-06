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
    public class POStatusCore : IPOStatusCore
    {
        private IPOStatusRepository POStatusRepo { get; set; }
        public POStatusCore(IPOStatusRepository poStatusRepo)
        {
            POStatusRepo = poStatusRepo;
        }

        public async Task<RequestResponse> GetPOStatusPg(int pageNum, int pageItem)
        {   
            var data = await POStatusRepo.GetPOStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await POStatusRepo.GetPOStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOStatusById(string poStatusId)
        {
            var data = await POStatusRepo.GetPOStatusById(poStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePOStatus(POStatusModel poStatus)
        {
            bool poStatusExists = await POStatusRepo.POStatusExists(poStatus.PoStatusId);
            if (poStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar PoStatusId exists.");
            }

            bool res = await POStatusRepo.CreatePOStatus(poStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdatePOStatus(POStatusModel poStatus)
        {
            bool res = await POStatusRepo.UpdatePOStatus(poStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeletePOStatus(string poStatusId)
        {
			// place item in use validator here

            bool res = await POStatusRepo.DeletePOStatus(poStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
