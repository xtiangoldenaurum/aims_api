using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IMovementTaskCore
    {
        Task<RequestResponse> GetMovementTaskPg(int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableMv(int pageNum, int pageItem);
        Task<RequestResponse> GetMovementTasksByInvMoveId(string invMoveId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableMvsById(string invMoveLineId, int pageNum, int pageItem);
        Task<RequestResponse> GetCancelableMvsByInvMoveId(string invMoveId, int pageNum, int pageItem);
        Task<RequestResponse> GetMovementTaskPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetMovementTaskById(string movementTaskId);
        //Task<RequestResponse> MovementTask(MovementTaskModelMod data);
        Task<RequestResponse> CreateMovementTask(MovementTaskModel movementTask);
        Task<RequestResponse> UpdateMovementTask(MovementTaskModel movementTask);
        Task<RequestResponse> CancelMovementTask(string movementTaskId, string userAccountId);
        Task<RequestResponse> DeleteMovementTask(string movementTaskId);
        Task<RequestResponse> ProceedMovementTask(CommitMovementTaskModel data);
    }
}
