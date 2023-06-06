using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvAdjustStatusCore
    {
        Task<RequestResponse> GetInvAdjustStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustStatusById(string invAdjustStatusId);
        Task<RequestResponse> CreateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus);
        Task<RequestResponse> UpdateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus);
        Task<RequestResponse> DeleteInvAdjustStatus(string invAdjustStatusId, string userAccountId);
    }
}
