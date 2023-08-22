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
    public interface IInvCountDetailRepository
    {
        //Task<InvCountDetailPagedMdl?> GetInvCountDetailByInvCountIDPaged(string invCountId, int pageNum, int pageItem);
        Task<InvCountDetailPagedMdlMod?> GetInvCountDetailByInvCountIDPagedMod(string invCountId, int pageNum, int pageItem);
        //Task<Pagination?> GetInvCountDetailPageDetail(IDbConnection db, string InvCountId, int pageNum, int pageItem, int rowCount);
        //Task<Pagination?> GetInvCountDetailPageDetailMod(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<InvCountDetailModel>> GetInvCountDetailPg(int pageNum, int pageItem);
        Task<IEnumerable<InvCountDetailModel>> GetInvCountDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvCountDetailModel> GetInvCountDetailById(string invCountLineId);
        Task<InvCountDetailModel> GetInvCountDetailByIdMod(IDbConnection db, string invCountLineId);
        //Task<string> GetInvCountDtlCancelMvUpdatedStatus(IDbConnection db, string invCountDetailId, string invCountId);
        Task<bool> InvCountDetailExists(string InvCountLineId);
        Task<bool> ChkInvCountDetailLock(string InvCountLineId);
        Task<InvCountDetailModel> LockInvCountDetail(IDbConnection db, string invCountLineId);
        Task<IEnumerable<InvCountDetailModel>> LockInvCountDetails(IDbConnection db, string invCountId);
        Task<bool> CreateInvCountDetail(InvCountDetailModel invCountDetail);
        Task<bool> CreateInvCountDetailMod(IDbConnection db, InvCountDetailModel invCountDetail);
        Task<bool> UpdateInvCountDetail(InvCountDetailModel invCountDetail);
        Task<bool> UpdateInvCountDetailMod(IDbConnection db, InvCountDetailModel invCountDetail, TranType tranTyp);
        Task<bool> DeleteInvCountDetail(string invCountLineId);
        Task<InvCountDetailDelResultCode> DeleteInvCountDetailMod(string invCountLineId, string userAccountId);
        //Task<bool> InvCountDetailMovable(string invCountLineId);
    }
}
