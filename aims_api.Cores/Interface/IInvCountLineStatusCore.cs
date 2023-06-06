using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvCountLineStatusCore
    {
        Task<RequestResponse> GetInvCountLineStatusPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountLineStatusPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountLineStatusById(string invCountLineStatusId);
        Task<RequestResponse> CreateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus);
        Task<RequestResponse> UpdateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus);
        Task<RequestResponse> DeleteInvCountLineStatus(string invCountLineStatusId);
    }
}
