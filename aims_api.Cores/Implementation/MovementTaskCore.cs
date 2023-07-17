using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class MovementTaskCore : IMovementTaskCore
    {
        private IMovementTaskRepository MovementTaskRepo { get; set; }
        public EnumHelper EnumHelper { get; set; }
        public MovementTaskCore(IMovementTaskRepository movementTaskRepository, EnumHelper enumHelper)
        {
            MovementTaskRepo = movementTaskRepository;
            EnumHelper = enumHelper;
        }
        public async Task<RequestResponse> GetMovementTaskPg(int pageNum, int pageItem)
        {
            var data = await MovementTaskRepo.GetMovementTaskPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableMv(int pageNum, int pageItem)
        {
            var data = await MovementTaskRepo.GetCancelableMv(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetMovementTasksByInvMoveId(string invMoveId, int pageNum, int pageItem)
        {
            var data = await MovementTaskRepo.GetMovementTasksByInvMoveId(invMoveId, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableMvsById(string invMoveLineId, int pageNum, int pageItem)
        {
            var data = await MovementTaskRepo.GetCancelableMvsById(invMoveLineId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetCancelableMvsByInvMoveId(string invMoveId, int pageNum, int pageItem)
        {
            var data = await MovementTaskRepo.GetCancelableMvsByInvMoveId(invMoveId, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetMovementTaskPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await MovementTaskRepo.GetMovementTaskPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetMovementTaskById(string movementTaskId)
        {
            var data = await MovementTaskRepo.GetMovementTaskById(movementTaskId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> MovementTask(MovementTaskModelMod data)
        {
            var res = await MovementTaskRepo.MovementTask(data);

            if (res != null)
            {
                string resMsg = await EnumHelper.GetDescription(res.ResultCode);
                if (res.ResultCode == MovementTaskResultCode.SUCCESS)
                {
                    return new RequestResponse(ResponseCode.SUCCESS, resMsg, res);
                }

                return new RequestResponse(ResponseCode.FAILED, resMsg, res);
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to process movement task.");
        }

        public async Task<RequestResponse> CreateMovementTask(MovementTaskModel movementTask)
        {
            bool movementTaskExists = await MovementTaskRepo.MovementTaskExists(movementTask.MovementTaskId);
            if (movementTaskExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar MovementTaskId exists.");
            }

            bool res = await MovementTaskRepo.CreateMovementTask(movementTask);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateMovementTask(MovementTaskModel movementTask)
        {
            bool res = await MovementTaskRepo.UpdateMovementTask(movementTask);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> CancelMovementTask(string movementTaskId, string userAccountId)
        {
            CancelMvResultCode res = await MovementTaskRepo.CancelMovementTask(movementTaskId, userAccountId);
            string resMsg = await EnumHelper.GetDescription(res);

            if (res == CancelMvResultCode.SUCCESS)
            {
                return new RequestResponse(ResponseCode.SUCCESS, resMsg, (res).ToString());
            }

            return new RequestResponse(ResponseCode.FAILED, resMsg, (res).ToString());
        }

        public async Task<RequestResponse> DeleteMovementTask(string movementTaskId)
        {
            // place item in use validator here

            bool res = await MovementTaskRepo.DeleteMovementTask(movementTaskId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
