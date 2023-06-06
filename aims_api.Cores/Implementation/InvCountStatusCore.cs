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
    public class InvCountStatusCore : IInvCountStatusCore
    {
        private IInvCountStatusRepository InvCountStatusRepo { get; set; }
        public InvCountStatusCore(IInvCountStatusRepository invCountStatusRepo)
        {
            InvCountStatusRepo = invCountStatusRepo;
        }

        public async Task<RequestResponse> GetInvCountStatusPg(int pageNum, int pageItem)
        {   
            var data = await InvCountStatusRepo.GetInvCountStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvCountStatusRepo.GetInvCountStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountStatusById(string invCountStatusId)
        {
            var data = await InvCountStatusRepo.GetInvCountStatusById(invCountStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvCountStatus(InvCountStatusModel invCountStatus)
        {
            bool invCountStatusExists = await InvCountStatusRepo.InvCountStatusExists(invCountStatus.InvCountStatusId);
            if (invCountStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvCountStatusId exists.");
            }

            bool res = await InvCountStatusRepo.CreateInvCountStatus(invCountStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvCountStatus(InvCountStatusModel invCountStatus)
        {
            bool res = await InvCountStatusRepo.UpdateInvCountStatus(invCountStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvCountStatus(string invCountStatusId)
        {
			// place item in use validator here

            bool res = await InvCountStatusRepo.DeleteInvCountStatus(invCountStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
