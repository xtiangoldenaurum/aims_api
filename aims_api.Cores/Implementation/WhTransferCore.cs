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
    public class WhTransferCore : IWhTransferCore
    {
        private IWhTransferRepository WhTransferRepo { get; set; }
        private IWhTransUserFieldRepository WhTransUFieldsRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public WhTransferCore(IWhTransferRepository whTransferRepo, IWhTransUserFieldRepository whTransUFieldsRepo, EnumHelper enumHelper)
        {
            WhTransferRepo = whTransferRepo;
            WhTransUFieldsRepo = whTransUFieldsRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetWhTransferSpecial(WhTransferFilteredMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            WhTransferPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.WhFromId) || 
                !string.IsNullOrEmpty(filter.CarrierId) || 
                filter.TransferDate != null || 
                !string.IsNullOrEmpty(filter.WhTransStatusId))
            {
                data = await WhTransferRepo.GetWhTransferFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await WhTransferRepo.GetWhTransSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await WhTransferRepo.GetWhTransferPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferForRcvPaged(int pageNum, int pageItem)
        {
            var data = await WhTransferRepo.GetWhTransferForRcvPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await WhTransferRepo.GetWhTransferPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferById(string whTransferId)
        {
            var data = await WhTransferRepo.GetWhTransferById(whTransferId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetWhTransferByIdMod(string whTransferId)
        {
            var whTransHeader = await WhTransferRepo.GetWhTransferById(whTransferId);
            var userFields = await WhTransUFieldsRepo.GetWhTransUserFieldById(whTransferId);

            if (whTransHeader != null)
            {
                var data = new WhTransferModelMod()
                {
                    whTransferHeader = whTransHeader,
                    WhTransferUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateWhTransferMod(WhTransferModelMod whTransfer)
        {
            var res = await WhTransferRepo.CreateWhTransferMod(whTransfer);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == WhTransferTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.WhTransferId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());
        }

        public async Task<RequestResponse> UpdateWhTransferMod(WhTransferModelMod whTransfer)
        {
            var res = await WhTransferRepo.UpdateWhTransferMod(whTransfer);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == WhTransferTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        //public async Task<RequestResponse> CreateWhTransfer(WhTransferModel whTransfer)
        //{
        //    bool whTransferExists = await WhTransferRepo.WhTransferExists(whTransfer.WhTransferId);
        //    if (whTransferExists)
        //    {
        //        return new RequestResponse(ResponseCode.FAILED, "Similar WhTransferId exists.");
        //    }

        //    bool res = await WhTransferRepo.CreateWhTransfer(whTransfer);
        //    if (res)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        //}

        //public async Task<RequestResponse> UpdateWhTransfer(WhTransferModel whTransfer)
        //{
        //    bool res = await WhTransferRepo.UpdateWhTransfer(whTransfer);
        //    if (res)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        //}

        public async Task<RequestResponse> DeleteWhTransfer(string whTransferId)
        {
			// place item in use validator here

            bool res = await WhTransferRepo.DeleteWhTransfer(whTransferId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> CancelReturns(string whTransId, string userAccountId)
        {
            var res = await WhTransferRepo.CancelWhTransfer(whTransId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelWhTransResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> ForceCancelWhTransfer(string whTransId, string userAccountId)
        {
            var res = await WhTransferRepo.ForceCancelWhTransfer(whTransId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelWhTransResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> GetDistinctWhTransFrom()
        {
            var data = await WhTransferRepo.GetDistinctWhTransFrom();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<string> DownloadWhTransferTemplate()
        {
            await Task.Delay(1000);

            return @"E:\Mark\AIMS\aims_api-main\aims_api-main\aims_api.Utilities\template\Inbound\WhTransfer_Template.xlsx";
        }

        public async Task<RequestResponse> ExportWhTransfer()
        {
            var data = await WhTransferRepo.ExportWhTransfer();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
