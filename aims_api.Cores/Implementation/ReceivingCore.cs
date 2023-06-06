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
    public class ReceivingCore : IReceivingCore
    {
        private IReceivingRepository ReceivingRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }

        public ReceivingCore(IReceivingRepository receivingRepo, EnumHelper enumHelper)
        {
            ReceivingRepo = receivingRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetReceivingPg(int pageNum, int pageItem)
        {
            var data = await ReceivingRepo.GetReceivingPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableRcvs(int pageNum, int pageItem)
        {
            var data = await ReceivingRepo.GetCancelableRcvs(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReceivesByPOId(string poId, int pageNum, int pageItem)
        {
            var data = await ReceivingRepo.GetReceivesByPOId(poId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableRcvsById(string poLineId, int pageNum, int pageItem)
        {
            var data = await ReceivingRepo.GetCancelableRcvsById(poLineId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableRcvsByPOId(string poId, int pageNum, int pageItem)
        {
            var data = await ReceivingRepo.GetCancelableRcvsByPOId(poId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ReceivingRepo.GetReceivingPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReceivingById(string receivingId)
        {
            var data = await ReceivingRepo.GetReceivingById(receivingId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> Receiving(ReceivingModelMod data)
        {
            var res = await ReceivingRepo.Receiving(data);

            if (res != null)
            {
                string resMsg = await EnumHelper.GetDescription(res.ResultCode);
                if (res.ResultCode == ReceivingResultCode.SUCCESS)
                {
                    return new RequestResponse(ResponseCode.SUCCESS, resMsg, res);
                }

                return new RequestResponse(ResponseCode.FAILED, resMsg, res);
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to process receiving.");
        }

        public async Task<RequestResponse> CreateReceiving(ReceivingModel receiving)
        {
            bool receivingExists = await ReceivingRepo.ReceivingExists(receiving.ReceivingId);
            if (receivingExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ReceivingId exists.");
            }

            bool res = await ReceivingRepo.CreateReceiving(receiving);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateReceiving(ReceivingModel receiving)
        {
            bool res = await ReceivingRepo.UpdateReceiving(receiving);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> CancelReceived(string receivingId, string userAccountId)
        {
            CancelRcvResultCode res = await ReceivingRepo.CancelReceived(receivingId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelRcvResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> DeleteReceiving(string receivingId)
        {
            // place item in use validator here

            bool res = await ReceivingRepo.DeleteReceiving(receivingId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> GetReceivingDetailByTrackId(string trackId)
        {
            var data = await ReceivingRepo.GetReceivingDetailByTrackId(trackId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

    }
}
