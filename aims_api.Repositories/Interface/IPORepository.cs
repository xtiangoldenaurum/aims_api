using aims_api.Enums;
using aims_api.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPORepository
    {
        Task<POPagedMdl?> GetPOPaged(int pageNum, int pageItem);
        Task<Pagination?> GetPOPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount);
        Task<POPagedMdl?> GetPOFilteredPaged(POFilteredMdl filter, int pageNum, int pageItem);
        Task<POPagedMdl?> GetPOForRcvPaged(int pageNum, int pageItem);
        Task<POPagedMdl?> GetPOSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<Pagination?> GetPOSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<POModel>> GetPOPg(int pageNum, int pageItem);
        Task<IEnumerable<POModel>> GetPOPgSrch(string searchKey, int pageNum, int pageItem);
        Task<POModel> GetPOById(string poId);
        Task<bool> POExists(string poId);
        Task<POModel> LockPO(IDbConnection db, string poId);
        Task<string?> GetPoUpdatedStatus(IDbConnection db, string poId);
        Task<POCreateTranResult> CreatePOMod(POModelMod po);
        Task<bool> SKUExistsInPO(IDbConnection db, string sku, string poId);
        Task<bool> POReceivable(string poId);
        Task<bool> CreatePO(IDbConnection db, POModel po);
        Task<POTranResultCode> UpdatePOMod(POModelMod po);
        Task<bool> UpdatePO(IDbConnection db, POModel po, TranType tranTyp);
        Task<bool> DeletePO(string poId);
        Task<CancelPOResultCode> CancelPO(string poId, string userAccountId);
        Task<CancelPOResultCode> ForceCancelPO(string poId, string userAccountId);
        Task<IEnumerable<POModel>> ExportPO();
        void CreatePO(POModel pos);
        //Task<IEnumerable<POModel>> ImportPOData(IFormFile file);
        Task<POTranResultCode> ImportPOData(IFormFile file);
    }
}
