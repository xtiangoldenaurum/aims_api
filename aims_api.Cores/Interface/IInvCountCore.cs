using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvCountCore
    {
        Task<RequestResponse> GetInvCountSpecial(InvCountFilteredMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountForCntPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountById(string invCountId);
        Task<RequestResponse> GetInvCountByIdMod(string invMoveId);
        Task<RequestResponse> CreateInvCountMod(InvCountModelMod invCount);
        Task<RequestResponse> UpdateInvCountMod(InvCountModelMod invCount);
        Task<RequestResponse> DeleteInvCount(string invMoveId);
        Task<RequestResponse> CancelInvCount(string invMoveId, string userAccountId);
        //Task<RequestResponse> ForceCancelInvCount(string invMoveId, string userAccountId);
    }
}
