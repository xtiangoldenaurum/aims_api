using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IReturnsCore
    {
        Task<RequestResponse> GetReturnsSpecial(ReturnsFilteredMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsForRcvPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsPg(int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetReturnsById(string returnsId);
        Task<RequestResponse> GetReturnsByIdMod(string returnsId);
        Task<RequestResponse> CreateReturnsMod(ReturnsModelMod returns);
        Task<RequestResponse> UpdateReturnsMod(ReturnsModelMod returns);
        //Task<RequestResponse> CreateReturns(ReturnsModel returns);
        //Task<RequestResponse> UpdateReturns(ReturnsModel returns);
        Task<RequestResponse> DeleteReturns(string returnsId);
        Task<RequestResponse> CancelReturns(string returnsId, string userAccountId);
        Task<RequestResponse> ForceCancelReturns(string returnsId, string userAccountId);
        Task<RequestResponse> GetDistinctStoreFrom();
        Task<string> DownloadRetTransferTemplate();
        Task<RequestResponse> ExportReturnsTransfer();
    }
}
