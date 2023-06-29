using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvMoveCore
    {
        Task<RequestResponse> GetInvMoveSpecial(InvMoveFilteredMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveForMvPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetInvMovePg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvMovePgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveById(string invMoveId);
        Task<RequestResponse> GetInvMoveByIdMod(string invMoveId);
        Task<RequestResponse> CreateInvMoveMod(InvMoveModelMod invMove);
        Task<RequestResponse> UpdateInvMoveMod(InvMoveModelMod invMove);
        Task<RequestResponse> DeleteInvMove(string invMoveId);
        Task<RequestResponse> CancelInvMove(string invMoveId, string userAccountId);
        Task<RequestResponse> ForceCancelInvMove(string invMoveId, string userAccountId);
    }
}
