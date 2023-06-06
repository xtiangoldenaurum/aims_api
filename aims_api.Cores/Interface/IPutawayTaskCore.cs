using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IPutawayTaskCore
    {
        Task<RequestResponse> GetPutawayTaskPg(int pageNum, int pageItem);
        Task<RequestResponse> GetPutawayTaskPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetPutawayTaskById(string putawayTaskId);
        Task<RequestResponse> CreatePutawayTask(PutawayTaskModel putawayTask);
        Task<RequestResponse> UpdatePutawayTask(PutawayTaskModel putawayTask);
        Task<RequestResponse> DeletePutawayTask(string putawayTaskId);
        Task<RequestResponse> PutawayQryTIDDetails(string trackId, string userAccountId);
        Task<RequestResponse> CommitPutaway(PutawayTaskProcModel data);
        Task<RequestResponse> QueryLPNPUtaway(string palletId);
        Task<RequestResponse> ProceedPalletPutaway(CommitPalletPutawayModel data);
    }
}
