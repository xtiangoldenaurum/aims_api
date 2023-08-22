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
    public interface IInvCountRepository
    {
        Task<InvCountPagedMdl?> GetInvCountPaged(int pageNum, int pageItem);
        //Task<Pagination?> GetInvCountPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount);
        Task<InvCountPagedMdl?> GetInvCountFilteredPaged(InvCountFilteredMdl filter, int pageNum, int pageItem);
        Task<InvCountPagedMdl?> GetInvCountForCntPaged(int pageNum, int pageItem);
        Task<InvCountPagedMdl?> GetInvCountSrchPaged(string searchKey, int pageNum, int pageItem);
        //Task<Pagination?> GetInvCountSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<InvCountModel>> GetInvCountPg(int pageNum, int pageItem);
        Task<IEnumerable<InvCountModel>> GetInvCountPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvCountModel> GetInvCountById(string invCountId);
        //Task<bool> InvCountExists(string invCountId);
        //Task<InvCountModel> LockInvCount(IDbConnection db, string invCountId);
        //Task<string?> GetInvCountUpdatedStatus(IDbConnection db, string invCountId);
        Task<InvCountCreateTranResult> CreateInvCountMod(InvCountModelMod invCount);
        Task<bool> InvIDExistsInInvCount(IDbConnection db, string inventoryId, string invCountId);
        Task<bool> CreateInvCount(IDbConnection db, InvCountModel invCount);
        Task<InvCountTranResultCode> UpdateInvCountMod(InvCountModelMod invCount);
        Task<bool> UpdateInvCount(IDbConnection db, InvCountModel invCount, TranType tranTyp);
        Task<bool> DeleteInvCount(string invCountId);
        Task<CancelInvCountResultCode> CancelInvCount(string invCountId, string userAccountId);
        //Task<CancelInvCountResultCode> ForceCancelInvCount(string invCountId, string userAccountId);
        //Task<bool> InvCountMovable(string invCountId);
    }
}
