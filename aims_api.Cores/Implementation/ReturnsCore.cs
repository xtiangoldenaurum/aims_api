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
    public class ReturnsCore : IReturnsCore
    {
        private IReturnsRepository ReturnsRepo { get; set; }
        private IReturnsUserFieldRepository ReturnsUFieldsRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public ReturnsCore(IReturnsRepository returnsRepo, IReturnsUserFieldRepository returnsUFieldRepo, EnumHelper enumHelper)
        {
            ReturnsRepo = returnsRepo;
            ReturnsUFieldsRepo = returnsUFieldRepo;
            EnumHelper = enumHelper;
        }

        public async Task<RequestResponse> GetReturnsSpecial(ReturnsFilteredMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            ReturnsPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.StoreId) ||
                !string.IsNullOrEmpty(filter.CarrierId) || 
                filter.ReturnDate != null || 
                !string.IsNullOrEmpty(filter.ReturnsStatusId))
            {
                data = await ReturnsRepo.GetReturnsFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await ReturnsRepo.GetReturnsSrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await ReturnsRepo.GetReturnsPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsForRcvPaged(int pageNum, int pageItem)
        {
            var data = await ReturnsRepo.GetReturnsForRcvPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsPg(int pageNum, int pageItem)
        {   
            var data = await ReturnsRepo.GetReturnsPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ReturnsRepo.GetReturnsPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsById(string returnsId)
        {
            var data = await ReturnsRepo.GetReturnsById(returnsId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetReturnsByIdMod(string returnsId)
        {
            var retHeader = await ReturnsRepo.GetReturnsById(returnsId);
            var userFields = await ReturnsUFieldsRepo.GetReturnsUserFieldById(returnsId);

            if (retHeader != null && userFields != null)
            {
                var data = new ReturnsModelMod()
                {
                    ReturnsHeader = retHeader,
                    ReturnsUfields = userFields
                };

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateReturnsMod(ReturnsModelMod returns)
        {
            var res = await ReturnsRepo.CreateReturnsMod(returns);
            string resMsg = await EnumHelper.GetDescription(res.ResultCode);

            if (res.ResultCode == ReturnsTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, res.ReturnsId);
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res.ResultCode).ToString());
        }

        public async Task<RequestResponse> UpdateReturnsMod(ReturnsModelMod returns)
        {
            var res = await ReturnsRepo.UpdateReturnsMod(returns);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == ReturnsTranResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        //public async Task<RequestResponse> CreateReturns(ReturnsModel returns)
        //{
        //    bool returnsExists = await ReturnsRepo.ReturnsExists(returns.ReturnsId);
        //    if (returnsExists)
        //    {
        //        return new RequestResponse(ResponseCode.FAILED, "Similar ReturnsId exists.");
        //    }

        //    bool res = await ReturnsRepo.CreateReturns(returns);
        //    if (res)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        //}

        //public async Task<RequestResponse> UpdateReturns(ReturnsModel returns)
        //{
        //    bool res = await ReturnsRepo.UpdateReturns(returns);
        //    if (res)
        //    {
        //        return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
        //    }

        //    return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        //}

        public async Task<RequestResponse> DeleteReturns(string returnsId)
        {
			// place item in use validator here

            bool res = await ReturnsRepo.DeleteReturns(returnsId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }

        public async Task<RequestResponse> CancelReturns(string returnsId, string userAccountId)
        {
            var res = await ReturnsRepo.CancelReturns(returnsId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelRetResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> ForceCancelReturns(string returnsId, string userAccountId)
        {
            var res = await ReturnsRepo.ForceCancelReturns(returnsId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelRetResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> GetDistinctStoreFrom()
        {
            var data = await ReturnsRepo.GetDistinctStoreFrom();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<string> DownloadRetTransferTemplate()
        {
            await Task.Delay(1000);

            return @"E:\Mark\AIMS\aims_api-main\aims_api-main\aims_api.Utilities\template\Inbound\ReturnsTransfer_Template.xlsx";
        }

        public async Task<RequestResponse> ExportReturnsTransfer()
        {
            var data = await ReturnsRepo.ExportReturnsTransfer();

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }
    }
}
