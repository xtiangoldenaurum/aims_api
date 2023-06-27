using aims_api.Enums;
using aims_api.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IReturnsRepository
    {
        Task<ReturnsPagedMdl?> GetReturnsPaged(int pageNum, int pageItem);
        Task<ReturnsPagedMdl?> GetReturnsFilteredPaged(ReturnsFilteredMdl filter, int pageNum, int pageItem);
        Task<ReturnsPagedMdl?> GetReturnsForRcvPaged(int pageNum, int pageItem);
        Task<ReturnsPagedMdl?> GetReturnsSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<IEnumerable<ReturnsModel>> GetReturnsPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ReturnsModel> GetReturnsById(string returnsId);
        Task<bool> ReturnsExists(string returnsId);
        Task<bool> ReturnsReceivable(string returnsId);
        Task<ReturnsModel> LockReturns(IDbConnection db, string returnsId);
        Task<string?> GetReturnsUpdatedStatus(IDbConnection db, string returnsId);
        Task<ReturnsCreateTranResult> CreateReturnsMod(ReturnsModelMod returns);
        Task<bool> CreateReturns(IDbConnection db, ReturnsModel returns);
        Task<ReturnsTranResultCode> UpdateReturnsMod(ReturnsModelMod returns);
        Task<bool> UpdateReturns(IDbConnection db, ReturnsModel returns, TranType tranTyp);
        Task<bool> DeleteReturns(string returnsId);
        Task<CancelRetResultCode> CancelReturns(string returnsId, string userAccountId);
        Task<CancelRetResultCode> ForceCancelReturns(string returnsId, string userAccountId);
        Task<IEnumerable<string>> GetDistinctStoreFrom();
        Task<IEnumerable<ReturnsModel>> GetExportReturnsTransfer();
        Task<ReturnsCreateTranResult> CreateBulkReturns(IFormFile file, string path);
    }
}
