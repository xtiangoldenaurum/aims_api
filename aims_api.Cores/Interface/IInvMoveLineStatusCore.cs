using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvMoveLineStatusCore
    {
        Task<RequestResponse> GetInvMoveLineStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvMoveLineStatusById(string invMoveLineStatusId);
        Task<RequestResponse> CreateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus);
        Task<RequestResponse> UpdateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus);
        Task<RequestResponse> DeleteInvMoveLineStatus(string invMoveLineStatusId);
    }
}
