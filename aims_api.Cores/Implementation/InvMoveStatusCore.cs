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
    public class InvMoveStatusCore : IInvMoveStatusCore
    {
        private IInvMoveStatusRepository InvMoveStatusRepo { get; set; }
        public InvMoveStatusCore(IInvMoveStatusRepository invMoveStatusRepo)
        {
            InvMoveStatusRepo = invMoveStatusRepo;
        }

        public async Task<RequestResponse> GetInvMoveStatusPg(int pageNum, int pageItem)
        {   
            var data = await InvMoveStatusRepo.GetInvMoveStatusPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveStatusPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await InvMoveStatusRepo.GetInvMoveStatusPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetInvMoveStatusById(string invMoveStatusId)
        {
            var data = await InvMoveStatusRepo.GetInvMoveStatusById(invMoveStatusId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateInvMoveStatus(InvMoveStatusModel invMoveStatus)
        {
            bool invMoveStatusExists = await InvMoveStatusRepo.InvMoveStatusExists(invMoveStatus.InvMoveStatusId);
            if (invMoveStatusExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar InvMoveStatusId exists.");
            }

            bool res = await InvMoveStatusRepo.CreateInvMoveStatus(invMoveStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateInvMoveStatus(InvMoveStatusModel invMoveStatus)
        {
            bool res = await InvMoveStatusRepo.UpdateInvMoveStatus(invMoveStatus);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteInvMoveStatus(string invMoveStatusId)
        {
			// place item in use validator here

            bool res = await InvMoveStatusRepo.DeleteInvMoveStatus(invMoveStatusId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
