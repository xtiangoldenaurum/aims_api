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
    public class PutawayTaskCore : IPutawayTaskCore
    {
        private IPutawayTaskRepository PutawayTaskRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public PutawayTaskCore(IPutawayTaskRepository putawayTaskRepo, EnumHelper enumHelper)
        {
            PutawayTaskRepo = putawayTaskRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetPutawayTaskPg(int pageNum, int pageItem)
        {   
            var data = await PutawayTaskRepo.GetPutawayTaskPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPutawayTaskPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await PutawayTaskRepo.GetPutawayTaskPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetPutawayTaskById(string putawayTaskId)
        {
            var data = await PutawayTaskRepo.GetPutawayTaskById(putawayTaskId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreatePutawayTask(PutawayTaskModel putawayTask)
        {
            bool putawayTaskExists = await PutawayTaskRepo.PutawayTaskExists(putawayTask.PutawayTaskId);
            if (putawayTaskExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar PutawayTaskId exists.");
            }

            bool res = await PutawayTaskRepo.CreatePutawayTask(putawayTask);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdatePutawayTask(PutawayTaskModel putawayTask)
        {
            bool res = await PutawayTaskRepo.UpdatePutawayTask(putawayTask);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeletePutawayTask(string putawayTaskId)
        {
			// place item in use validator here

            bool res = await PutawayTaskRepo.DeletePutawayTask(putawayTaskId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> PutawayQryTIDDetails(string trackId, string userAccountId)
        {
            var res = await PutawayTaskRepo.PutawayQryTIDDetails(trackId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == PutawayResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.Data);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }

        public async Task<RequestResponse> CommitPutaway(PutawayTaskProcModel data)
        {
            var res = await PutawayTaskRepo.CommitPutaway(data);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == PutawayResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }

        public async Task<RequestResponse> QueryLPNPUtaway(string palletId)
        {
            var res = await PutawayTaskRepo.QueryLPNPUtaway(palletId);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == PalletPutawayResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.Data);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }

        public async Task<RequestResponse> ProceedPalletPutaway(CommitPalletPutawayModel data)
        {
            var res = await PutawayTaskRepo.ProceedPalletPutaway(data);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == PalletPutawayResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg);
            }

            // complete message in case specific TID issue
            if (res.ResultCode == PalletPutawayResultCode.SPECIFICTIDISSUE &&
                !string.IsNullOrEmpty(res.ConlictMsg))
            {
                resMsg += res.ConlictMsg;
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg);
        }
    }
}
