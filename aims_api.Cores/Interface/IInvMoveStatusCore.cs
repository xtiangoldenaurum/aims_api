using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvMoveStatusCore
    {
        Task<RequestResponse> GetInvMoveStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveStatusById(string invMoveStatusId);
        Task<RequestResponse> CreateInvMoveStatus(InvMoveStatusModel invMoveStatus);
        Task<RequestResponse> UpdateInvMoveStatus(InvMoveStatusModel invMoveStatus);
        Task<RequestResponse> DeleteInvMoveStatus(string invMoveStatusId);
    }
}
