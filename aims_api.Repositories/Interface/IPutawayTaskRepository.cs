using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IPutawayTaskRepository
    {
        Task<IEnumerable<PutawayTaskModel>> GetPutawayTaskPg(int pageNum, int pageItem);
        Task<IEnumerable<PutawayTaskModel>> GetPutawayTaskPgSrch(string searchKey, int pageNum, int pageItem);
        Task<PutawayTaskModel> GetPutawayTaskById(string putawayTaskId);
        Task<PutawayTaskModel> GetPutawayTaskByIdMod(IDbConnection db, string putawayTaskId);
        Task<bool> PutawayTaskExists(string putawayTaskId);
        Task<PutawayTaskModel> LockPutawayTaskDtl(IDbConnection db, string receivingId);
        Task<bool> CreatePutawayTask(PutawayTaskModel putawayTask);
        Task<bool> CreatePutawayTaskMod(IDbConnection db, PutawayTaskModel putawayTask, TranType tranTyp);
        Task<bool> UpdatePutawayTask(PutawayTaskModel putawayTask);
        Task<bool> SetPutawayStatus(string putawayTaskId, string putawaytStatusId, string userAccountId);
        Task<bool> SetPutawayStatus(IDbConnection db, string putawayTaskId, string putawayStatusId, string userAccountId, TranType tranTyp);
        Task<bool> DeletePutawayTask(string putawayTaskId);
        Task<bool> HasPendingPutawayTaskPO(IDbConnection db, string poId);
        Task<bool> HasPendingPutawayTaskRet(IDbConnection db, string returnsId);
        Task<PutawayResultModel> PutawayQryTIDDetails(string trackId, string userAccountId);
        Task<PutawayResultModel> CommitPutaway(PutawayTaskProcModel data);
        Task<PutawayResultCode> FullPutawayPO(IDbConnection db, PutawayContainerModel data);
        Task<PutawayResultCode> FullPutawayReturns(IDbConnection db, PutawayContainerModel data);
        Task<PutawayResultCode> FullPutawayWhTrans(IDbConnection db, PutawayContainerModel data);
        Task<PutawayResultCode> PartialPutaway(IDbConnection db, PutawayContainerModel data);
        Task<PutawayResultCode> CommitPartialPutaway(IDbConnection db, PartialPutawayRefIdModel refIds, PutawayTaskProcModel winData);
        Task<QryPalletPutawayResult> QueryLPNPUtaway(string palletId);
        Task<PalletPutawayResult> ProceedPalletPutaway(CommitPalletPutawayModel data);
        Task<PutawayResultModel> LPNPutawayQryTIDDetails(string trackId);
        Task<PutawayResultModel> CommitPalletPutaway(IDbConnection db, PutawayTaskProcModel data);
        Task<string?> DefineTranTypeByDocId(IDbConnection db, string docLineId);
    }
}
