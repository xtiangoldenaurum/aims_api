using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvCountStatusCore
    {
        Task<RequestResponse> GetInvCountStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountStatusById(string invCountStatusId);
        Task<RequestResponse> CreateInvCountStatus(InvCountStatusModel invCountStatus);
        Task<RequestResponse> UpdateInvCountStatus(InvCountStatusModel invCountStatus);
        Task<RequestResponse> DeleteInvCountStatus(string invCountStatusId);
    }
}
