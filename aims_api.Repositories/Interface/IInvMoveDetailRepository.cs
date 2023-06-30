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
    public interface IInvMoveDetailRepository
    {
        Task<InvMoveDetailPagedMdl?> GetInvMoveDetailByInvMoveIDPaged(string invMoveId, int pageNum, int pageItem);
        Task<InvMoveDetailPagedMdlMod?> GetInvMoveDetailByInvMoveIDPagedMod(string invMoveId, int pageNum, int pageItem);
        Task<Pagination?> GetInvMoveDetailPageDetail(IDbConnection db, string InvMoveId, int pageNum, int pageItem, int rowCount);
        Task<Pagination?> GetInvMoveDetailPageDetailMod(IDbConnection db, string poId, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<InvMoveDetailModel>> GetInvMoveDetailPg(int pageNum, int pageItem);
        Task<IEnumerable<InvMoveDetailModel>> GetInvMoveDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvMoveDetailModel> GetInvMoveDetailById(string invMoveLineId);
        Task<InvMoveDetailModel> GetInvMoveDetailByIdMod(IDbConnection db, string invMoveLineId);
        Task<string> GetInvMoveDtlCancelMvUpdatedStatus(IDbConnection db, string invMoveDetailId, string invMoveId);
        Task<bool> InvMoveDetailExists(string InvMoveLineId);
        Task<bool> ChkInvMoveDetailLock(string InvMoveLineId);
        Task<InvMoveDetailModel> LockInvMoveDetail(IDbConnection db, string invMoveLineId);
        Task<IEnumerable<InvMoveDetailModel>> LockInvMoveDetails(IDbConnection db, string invMoveId);
        Task<bool> CreateInvMoveDetail(InvMoveDetailModel invMoveDetail);
        Task<bool> CreateInvMoveDetailMod(IDbConnection db, InvMoveDetailModel invMoveDetail);
        Task<bool> UpdateInvMoveDetail(InvMoveDetailModel invMoveDetail);
        Task<bool> UpdateInvMoveDetailMod(IDbConnection db, InvMoveDetailModel invMoveDetail, TranType tranTyp);
        Task<bool> DeleteInvMoveDetail(string invMoveLineId);
        Task<InvMoveDetailDelResultCode> DeleteInvMoveDetailMod(string invMoveLineId, string userAccountId);
    }
}
