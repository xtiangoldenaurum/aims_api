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
    public class POLineStatusCore : IPOLineStatusCore
    {
        private IPOLineStatusRepository POLineStatusRepo { get; set; }
        public POLineStatusCore(IPOLineStatusRepository poLineStatusRepo)
        {
            POLineStatusRepo = poLineStatusRepo;
        }

        public async Task<RequestResponse> GetPOLineStatusPg(int pageNum, int pageItem)
        {   
            var data = await POLineStatusRepo.GetPOLineStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await POLineStatusRepo.GetPOLineStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPOLineStatusById(string poLineStatusId)
        {
            var data = await POLineStatusRepo.GetPOLineStatusById(poLineStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePOLineStatus(POLineStatusModel poLineStatus)
        {
            bool poLineStatusExists = await POLineStatusRepo.POLineStatusExists(poLineStatus.PoLineStatusId);
            if (poLineStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar PoLineStatusId exists.");
            }

            bool res = await POLineStatusRepo.CreatePOLineStatus(poLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdatePOLineStatus(POLineStatusModel poLineStatus)
        {
            bool res = await POLineStatusRepo.UpdatePOLineStatus(poLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeletePOLineStatus(string poLineStatusId)
        {
			// place item in use validator here

            bool res = await POLineStatusRepo.DeletePOLineStatus(poLineStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
