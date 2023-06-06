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
    public class InvAdjustStatusCore : IInvAdjustStatusCore
    {
        private IInvAdjustStatusRepository InvAdjustStatusRepo { get; set; }
        public InvAdjustStatusCore(IInvAdjustStatusRepository invAdjustStatusRepo)
        {
            InvAdjustStatusRepo = invAdjustStatusRepo;
        }

        public async Task<RequestResponse> GetInvAdjustStatusPg(int pageNum, int pageItem)
        {   
            var data = await InvAdjustStatusRepo.GetInvAdjustStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvAdjustStatusRepo.GetInvAdjustStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustStatusById(string invAdjustStatusId)
        {
            var data = await InvAdjustStatusRepo.GetInvAdjustStatusById(invAdjustStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus)
        {
            bool invAdjustStatusExists = await InvAdjustStatusRepo.InvAdjustStatusExists(invAdjustStatus.InvAdjustStatusId);
            if (invAdjustStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvAdjustStatusId exists.");
            }

            bool res = await InvAdjustStatusRepo.CreateInvAdjustStatus(invAdjustStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus)
        {
            bool res = await InvAdjustStatusRepo.UpdateInvAdjustStatus(invAdjustStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvAdjustStatus(string invAdjustStatusId, string userAccountId)
        {
			// place item in use validator here

            bool res = await InvAdjustStatusRepo.DeleteInvAdjustStatus(invAdjustStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
