using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IWhTransferCore
    {
        Task<RequestResponse> GetWhTransferSpecial(WhTransferFilteredMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetWhTransferForRcvPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetWhTransferPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetWhTransferById(string whTransferId);
        Task<RequestResponse> GetWhTransferByIdMod(string whTransferId);
        Task<RequestResponse> CreateWhTransferMod(WhTransferModelMod whTransfer);
        Task<RequestResponse> UpdateWhTransferMod(WhTransferModelMod whTransfer);
        Task<RequestResponse> DeleteWhTransfer(string whTransferId);
        Task<RequestResponse> CancelReturns(string whTransId, string userAccountId);
        Task<RequestResponse> ForceCancelWhTransfer(string whTransId, string userAccountId);
        Task<RequestResponse> GetDistinctWhTransFrom();
        Task<string> GetWhTransferTemplate();
        Task<RequestResponse> GetExportWhTransfer();
        Task<RequestResponse> CreateBulkWhTransfer(IFormFile file, string path);
    }
}
