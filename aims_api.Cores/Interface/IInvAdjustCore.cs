using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvAdjustCore
    {
        Task<RequestResponse> GetInvAdjustSpecial(InvAdjustFilteredMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustForAdjPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustById(string invAdjustId);
        Task<RequestResponse> GetInvAdjustByIdMod(string invAdjustId);
        Task<RequestResponse> CreateInvAdjustMod(InvAdjustModelMod invAdjust);
        Task<RequestResponse> UpdateInvAdjustMod(InvAdjustModelMod invAdjust);
        Task<RequestResponse> DeleteInvAdjust(string invAdjustId);
        Task<RequestResponse> CancelInvAdjust(string invAdjustId, string userAccountId);
        Task<RequestResponse> ForceCancelInvAdjust(string invAdjustId, string userAccountId);
        Task<RequestResponse> UpdateInvAdjustApprovedMod(string invAdjustId, string userAccountId);
    }
}
