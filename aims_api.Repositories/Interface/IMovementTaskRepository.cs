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
    public interface IMovementTaskRepository
    {
        Task<IEnumerable<MovementTaskModel>> GetMovementTaskPg(int pageNum, int pageItem);
        Task<MovementTaskPagedMdl?> GetCancelableMv(int pageNum, int pageItem);
        Task<MovementTaskPagedMdl?> GetMovementTasksByInvMoveId(string poId, int pageNum, int pageItem);
        Task<IEnumerable<InvMoveMovementTaskDetailModel>?> GetCancelableMvsById(string invMoveLineId, int pageNum, int pageItem);
        Task<IEnumerable<InvMoveMovementTaskDetailModel>?> GetCancelableMvsByInvMoveId(string invMoveId, int pageNum, int pageItem);
        Task<IEnumerable<MovementTaskModel>> GetMovementTaskPgSrch(string searchKey, int pageNum, int pageItem);
        Task<MovementTaskModel> GetMovementTaskById(string movementTaskId);
        Task<MovementTaskModel> GetMovementTaskByIdMod(IDbConnection db, string movementTaskId);
        Task<bool> MovementTaskExists(string movementTaskId);
        //Task<MovementTaskResultModel> MovementTask(MovementTaskModelMod data);
        Task<bool> CreateMovementTask(MovementTaskModel movementTask);
        Task<bool> CreateMovementTaskMod(IDbConnection db, MovementTaskModel movementTask, TranType tranTyp);
        Task<bool> UpdateMovementTask(MovementTaskModel movementTask);
        Task<CancelMvResultCode> CancelMovementTask(string movementTaskId, string userAccountId);
        Task<MovementTaskModel> LockMovementTaskDetail(IDbConnection db, string movementTaskId);
        Task<bool> SetMovementTaskStatus(string movementTaskId, string movementTaskStatus);
        Task<bool> SetMovementTaskStatus(IDbConnection db, string movementTaskId, string movementTaskStatus, TranType tranTyp, string userAccountId);
        Task<bool> DeleteMovementTask(string movementTaskId);
        Task<MovementTaskModel> LockMovementTaskDetailRefMulti(IDbConnection db, string invMoveLineId, string inventoryId);
        Task<bool> HasPendingMovementTask(IDbConnection db, string invMoveId);
        Task<MovementTaskResultCode> PartialMovement(IDbConnection db, MovementContainerModel data);
        //Task<MovementTaskResultCode> CommitPartialMovement(IDbConnection db, PartialMovementRefIdModel refIds, MovementTaskProcModel winData);
        Task<MovementTaskResultCode> FullMovementInvMove(IDbConnection db, MovementContainerModel data);
        //Task<MovementTaskResultModel> CommitMovement(IDbConnection db, MovementTaskProcModel data);
        Task<MovementTaskResultModel> ProceedMovementTask(CommitMovementTaskModel data);
        Task<string?> DefineTranTypeByDocId(IDbConnection db, string docLineId);
    }
}
