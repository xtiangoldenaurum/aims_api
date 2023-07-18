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
    public interface IInvMoveRepository
    {
        Task<InvMovePagedMdl?> GetInvMovePaged(int pageNum, int pageItem);
        Task<Pagination?> GetInvMovePageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount);
        Task<InvMovePagedMdl?> GetInvMoveFilteredPaged(InvMoveFilteredMdl filter, int pageNum, int pageItem);
        Task<InvMovePagedMdl?> GetInvMoveForMvPaged(int pageNum, int pageItem);
        Task<InvMovePagedMdl?> GetInvMoveSrchPaged(string searchKey, int pageNum, int pageItem);
        Task<Pagination?> GetInvMoveSrchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount);
        Task<IEnumerable<InvMoveModel>> GetInvMovePg(int pageNum, int pageItem);
        Task<IEnumerable<InvMoveModel>> GetInvMovePgSrch(string searchKey, int pageNum, int pageItem);
        Task<InvMoveModel> GetInvMoveById(string invMoveId);
        Task<bool> InvMoveExists(string invMoveId);
        Task<InvMoveModel> LockInvMove(IDbConnection db, string invMoveId);
        Task<string?> GetInvMoveUpdatedStatus(IDbConnection db, string invMoveId);
        Task<InvMoveCreateTranResult> CreateInvMoveMod(InvMoveModelMod invMove);
        Task<bool> InvIDExistsInInvMove(IDbConnection db, string inventoryId, string invMoveId);
        Task<bool> CreateInvMove(IDbConnection db, InvMoveModel invMove);
        Task<InvMoveTranResultCode> UpdateInvMoveMod(InvMoveModelMod invMove);
        Task<bool> UpdateInvMove(IDbConnection db, InvMoveModel invMove, TranType tranTyp);
        Task<bool> DeleteInvMove(string invMoveId);
        Task<CancelInvMoveResultCode> CancelInvMove(string invMoveId, string userAccountId);
        Task<CancelInvMoveResultCode> ForceCancelInvMove(string invMoveId, string userAccountId);
        Task<bool> InvMoveMovable(string invMoveId);
    }
}
