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
    public class RetReceivingCore : IRetReceivingCore
    {
        private IRetReceivingRepository RetReceivingRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }

        public RetReceivingCore(IRetReceivingRepository retReceivingRepo, EnumHelper enumHelper)
        {
            RetReceivingRepo = retReceivingRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetReceivesByReturnsId(string returnsId, int pageNum, int pageItem)
        {
            var data = await RetReceivingRepo.GetReceivesByReturnsId(returnsId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableRcvsById(string retLineId, int pageNum, int pageItem)
        {
            var data = await RetReceivingRepo.GetCancelableRcvsById(retLineId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableRcvsByRetId(string returnsId, int pageNum, int pageItem)
        {
            var data = await RetReceivingRepo.GetCancelableRcvsByRetId(returnsId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReceivingPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await RetReceivingRepo.GetReceivingPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReceivingById(string receivingId)
        {
            var data = await RetReceivingRepo.GetReceivingById(receivingId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> Receiving(RetReceivingModelMod data)
        {
            var res = await RetReceivingRepo.Receiving(data);

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

        public async Task<RequestResponse> CancelReceived(string receivingId, string userAccountId)
        {
            CancelRcvResultCode res = await RetReceivingRepo.CancelReceived(receivingId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelRcvResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }
    }
}
