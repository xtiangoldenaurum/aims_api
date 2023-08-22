using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvCountDetailCore
    {
        //Task<RequestResponse> GetInvCountDetailByInvCountIDPaged(string invCountId, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountDetailByInvCountIDPagedMod(string invCountId, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountDetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvCountDetailById(string invCountLineId);
        Task<RequestResponse> CreateInvCountDetail(InvCountDetailModel invCountDetail);
        Task<RequestResponse> UpdateInvCountDetail(InvCountDetailModel invCountDetail);
        Task<RequestResponse> DeleteInvCountDetail(string invCountLineId);
        Task<RequestResponse> DeleteInvCountDetailMod(string invCountLineId, string userAccountId);
    }
}
