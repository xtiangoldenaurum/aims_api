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
    public class InvCountLineStatusCore : IInvCountLineStatusCore
    {
        private IInvCountLineStatusRepository InvCountLineStatusRepo { get; set; }
        public InvCountLineStatusCore(IInvCountLineStatusRepository invCountLineStatusRepo)
        {
            InvCountLineStatusRepo = invCountLineStatusRepo;
        }

        public async Task<RequestResponse> GetInvCountLineStatusPg(int pageNum, int pageItem)
        {   
            var data = await InvCountLineStatusRepo.GetInvCountLineStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvCountLineStatusRepo.GetInvCountLineStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvCountLineStatusById(string invCountLineStatusId)
        {
            var data = await InvCountLineStatusRepo.GetInvCountLineStatusById(invCountLineStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus)
        {
            bool invCountLineStatusExists = await InvCountLineStatusRepo.InvCountLineStatusExists(invCountLineStatus.InvCountLineStatusId);
            if (invCountLineStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvCountLineStatusId exists.");
            }

            bool res = await InvCountLineStatusRepo.CreateInvCountLineStatus(invCountLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus)
        {
            bool res = await InvCountLineStatusRepo.UpdateInvCountLineStatus(invCountLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvCountLineStatus(string invCountLineStatusId)
        {
			// place item in use validator here

            bool res = await InvCountLineStatusRepo.DeleteInvCountLineStatus(invCountLineStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
