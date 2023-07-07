using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IInvAdjustRepository
    {
        Task<InvAdjustPagedMdl?> GetInvAdjustPaged(int pageNum, int pageItem);
        Task<Pagination?> GetInvAdjustPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount);
        Task<InvAdjustPagedMdl?> GetInvAdjustFilteredPaged(InvAdjustFilteredMdl filter, int pageNum, int pageItem);
        Task<InvAdjustPagedMdl?> GetInvAdjustForAdjPaged(int pageNum, int pageItem);
        Task<InvAdjustPagedMdl?> GetInvAdjustSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<Pagination?> GetInvAdjustSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<InvAdjustModel>> GetInvAdjustPg(int pageNum, int pageItem);
        Task<IEnumerable<InvAdjustModel>> GetInvAdjustPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvAdjustModel> GetInvAdjustById(string invAdjustId);
        Task<bool> InvAdjustExists(string invAdjustId);
        Task<InvAdjustModel> LockInvAdjust(IDbConnection db, string invAdjustId);
        Task<string?> GetInvAdjustUpdatedStatus(IDbConnection db, string invAdjustId);
        Task<InvAdjustCreateTranResult> CreateInvAdjustMod(InvAdjustModelMod invAdjust);
        Task<bool> SKUExistsInInvAdjust(IDbConnection db, string inventoryId, string invAdjustId);
        Task<bool> CreateInvAdjust(IDbConnection db, InvAdjustModel invAdjust);
        Task<InvAdjustTranResultCode> UpdateInvAdjustMod(InvAdjustModelMod invAdjust);
        Task<bool> UpdateInvAdjust(IDbConnection db, InvAdjustModel invAdjust, TranType tranTyp);
        Task<bool> DeleteInvAdjust(string invAdjustId);
        Task<CancelInvAdjustResultCode> CancelInvAdjust(string invAdjustId, string userAccountId);
        Task<CancelInvAdjustResultCode> ForceCancelInvAdjust(string invAdjustId, string userAccountId);
        Task<InvAdjustTranResultCode> UpdateInvAdjustApprovedMod(string invAdjustId, string userAccountId);
    }
}
