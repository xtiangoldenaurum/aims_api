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
    public class InvMoveLineStatusCore : IInvMoveLineStatusCore
    {
        private IInvMoveLineStatusRepository InvMoveLineStatusRepo { get; set; }
        public InvMoveLineStatusCore(IInvMoveLineStatusRepository invMoveLineStatusRepo)
        {
            InvMoveLineStatusRepo = invMoveLineStatusRepo;
        }

        public async Task<RequestResponse> GetInvMoveLineStatusPg(int pageNum, int pageItem)
        {   
            var data = await InvMoveLineStatusRepo.GetInvMoveLineStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveLineStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvMoveLineStatusRepo.GetInvMoveLineStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveLineStatusById(string invMoveLineStatusId)
        {
            var data = await InvMoveLineStatusRepo.GetInvMoveLineStatusById(invMoveLineStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus)
        {
            bool invMoveLineStatusExists = await InvMoveLineStatusRepo.InvMoveLineStatusExists(invMoveLineStatus.InvMoveLineStatusId);
            if (invMoveLineStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvMoveLineStatusId exists.");
            }

            bool res = await InvMoveLineStatusRepo.CreateInvMoveLineStatus(invMoveLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus)
        {
            bool res = await InvMoveLineStatusRepo.UpdateInvMoveLineStatus(invMoveLineStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvMoveLineStatus(string invMoveLineStatusId)
        {
			// place item in use validator here

            bool res = await InvMoveLineStatusRepo.DeleteInvMoveLineStatus(invMoveLineStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
