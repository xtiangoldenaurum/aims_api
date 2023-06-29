using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvMoveDetailCore
    {
        Task<RequestResponse> GetInvMoveDetailByInvMoveIDPaged(string invMoveId, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveDetailByInvMoveIDPagedMod(string invMoveId, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveDetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveDetailById(string invMoveLineId);
        Task<RequestResponse> CreateInvMoveDetail(InvMoveDetailModel invMoveDetail);
        Task<RequestResponse> UpdateInvMoveDetail(InvMoveDetailModel invMoveDetail);
        Task<RequestResponse> DeleteInvMoveDetail(string invMoveLineId);
        Task<RequestResponse> DeleteInvMoveDetailMod(string invMoveLineId, string userAccountId);
    }
}
