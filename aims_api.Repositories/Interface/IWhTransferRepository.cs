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
    public interface IWhTransferRepository
    {
        Task<WhTransferPagedMdl?> GetWhTransferPaged(int pageNum, int pageItem);
        Task<WhTransferPagedMdl?> GetWhTransferFilteredPaged(WhTransferFilteredMdl filter, int pageNum, int pageItem);
        Task<WhTransferPagedMdl?> GetWhTransferForRcvPaged(int pageNum, int pageItem);
        Task<WhTransferPagedMdl?> GetWhTransSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<IEnumerable<WhTransferModel>> GetWhTransferPgSrch(string searchKey, int pageNum, int pageItem);
        Task<WhTransferModel> GetWhTransferById(string whTransferId);
        Task<bool> WhTransferExists(string whTransferId);
        Task<bool> WhTransferReceivable(string whTransId);
        Task<WhTransferModel> LockWhTransfer(IDbConnection db, string whTransId);
        Task<string?> GetWhTransUpdatedStatus(IDbConnection db, string whTransId);
        Task<WhTransCreateTranResult> CreateWhTransferMod(WhTransferModelMod whTransfer);
        Task<bool> CreateWhTransfer(IDbConnection db, WhTransferModel whTransfer);
        Task<WhTransferTranResultCode> UpdateWhTransferMod(WhTransferModelMod whTransfer);
        Task<bool> UpdateWhTransfer(IDbConnection db, WhTransferModel whTransfer, TranType tranType);
        Task<bool> DeleteWhTransfer(string whTransferId);
        Task<CancelWhTransResultCode> CancelWhTransfer(string whTransId, string userAccountId);
        Task<CancelWhTransResultCode> ForceCancelWhTransfer(string whTransId, string userAccountId);
        Task<IEnumerable<string>> GetDistinctWhTransFrom();
        Task<IEnumerable<WhTransferModel>> GetExportWhTransfer();
        Task<WhTransCreateTranResult> CreateBulkWhTransfer(IFormFile file, string path);
    }
}
