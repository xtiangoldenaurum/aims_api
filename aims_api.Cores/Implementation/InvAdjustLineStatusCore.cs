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
    public class InvAdjustLineStatusCore : IInvAdjustLineStatusCore
    {
        private IInvAdjustLineStatusRepository InvAdjustLineStatusRepo { get; set; }
        public InvAdjustLineStatusCore(IInvAdjustLineStatusRepository invAdjustLineStatusRepo)
        {
            InvAdjustLineStatusRepo = invAdjustLineStatusRepo;
        }

        public async Task<RequestResponse> GetInvAdjustLineStatusPg(int pageNum, int pageItem)
        {   
            var data = await InvAdjustLineStatusRepo.GetInvAdjustLineStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvAdjustLineStatusRepo.GetInvAdjustLineStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvAdjustLineStatusById(string invAdjustLineStatusId)
        {
            var data = await InvAdjustLineStatusRepo.GetInvAdjustLineStatusById(invAdjustLineStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus)
        {
            bool invAdjustLineStatusExists = await InvAdjustLineStatusRepo.InvAdjustLineStatusExists(invAdjustLineStatus.InvAdjustLineStatusId);
            if (invAdjustLineStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvAdjustLineStatusId exists.");
            }

            bool res = await InvAdjustLineStatusRepo.CreateInvAdjustLineStatus(invAdjustLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus)
        {
            bool res = await InvAdjustLineStatusRepo.UpdateInvAdjustLineStatus(invAdjustLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvAdjustLineStatus(string invAdjustLineStatusId, string userAccountId)
        {
			// place item in use validator here

            bool res = await InvAdjustLineStatusRepo.DeleteInvAdjustLineStatus(invAdjustLineStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
