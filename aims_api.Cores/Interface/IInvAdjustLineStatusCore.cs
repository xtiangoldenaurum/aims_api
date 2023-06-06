using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvAdjustLineStatusCore
    {
        Task<RequestResponse> GetInvAdjustLineStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustLineStatusById(string invAdjustLineStatusId);
        Task<RequestResponse> CreateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus);
        Task<RequestResponse> UpdateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus);
        Task<RequestResponse> DeleteInvAdjustLineStatus(string invAdjustLineStatusId, string userAccountId);
    }
}
