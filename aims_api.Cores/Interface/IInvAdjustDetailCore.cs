using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IInvAdjustDetailCore
    {
        Task<RequestResponse> GetInvAdjustDetailByInvAdjustIDPaged(string invAdjustId, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustDetailByInvAdjustIDPagedMod(string invAdjustId, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustDetailPg(int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustDetailPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetInvAdjustDetailById(string invAdjustLineId);
        Task<RequestResponse> CreateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail);
        Task<RequestResponse> UpdateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail);
        Task<RequestResponse> DeleteInvAdjustDetail(string invAdjustLineId);
        Task<RequestResponse> DeleteInvAdjustDetailMod(string invAdjustLineId, string userAccountId);
    }
}
