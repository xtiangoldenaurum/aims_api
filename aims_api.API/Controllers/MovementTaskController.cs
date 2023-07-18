using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MovementTaskController : ControllerBase
    {
        private IMovementTaskCore MovementTaskCore { get; set; }
        DataValidator DataValidator;

        public MovementTaskController(IMovementTaskCore movementTaskCore)
        {
            MovementTaskCore = movementTaskCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getmovementtaskpg")]
        public async Task<ActionResult> GetMovementTaskPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await MovementTaskCore.GetMovementTaskPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getcancelablemv")]
        public async Task<ActionResult> GetCancelableMv(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await MovementTaskCore.GetCancelableMv(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmovementtasksbypoid")]
        public async Task<ActionResult> GetMovementTasksByInvMoveId(string invMoveId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.GetMovementTasksByInvMoveId(invMoveId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getcancelablemvsbyid")]
        public async Task<ActionResult> GetCancelableMvsById(string invMoveLineId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveLineId))
                {
                    await DataValidator.AddErrorField("invMoveLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.GetCancelableMvsById(invMoveLineId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getcancelablemvsbyinvmoveid")]
        public async Task<ActionResult> GetCancelableMvsByInvMoveId(string invMoveId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.GetCancelableMvsByInvMoveId(invMoveId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmovementtaskpgsrch")]
        public async Task<ActionResult> GetMovementTaskPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(searchKey))
                {
                    await DataValidator.AddErrorField("searchKey");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.GetMovementTaskPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmovementtaskbyid")]
        public async Task<ActionResult> GetMovementTaskById(string movementTaskId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTaskId))
                {
                    await DataValidator.AddErrorField("movementTaskId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.GetMovementTaskById(movementTaskId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        //[HttpPost("movementtask")]
        //public async Task<ActionResult> MovementTask(MovementTaskModelMod data)
        //{
        //    try
        //    {
        //        return Ok(await MovementTaskCore.MovementTask(data));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}


        [HttpPost("proceedmovementtask")]
        public async Task<ActionResult> ProceedMovementTask(CommitMovementTaskModel data)
        {
            try
            {
                if (data == null)
                {
                    await DataValidator.AddErrorField("data");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.ProceedMovementTask(data));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createmovementtask")]
        public async Task<ActionResult> CreateMovementTask(MovementTaskModel movementTask)
        {
            try
            {
                if (movementTask == null)
                {
                    await DataValidator.AddErrorField("movementTask");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.CreateMovementTask(movementTask));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatemovementtask")]
        public async Task<ActionResult> UpdateMovementTask(MovementTaskModel movementTask)
        {
            try
            {
                if (movementTask == null)
                {
                    await DataValidator.AddErrorField("movementTask");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.UpdateMovementTask(movementTask));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("cancelreceived")]
        public async Task<ActionResult> CancelMovementTask(string movementTaskId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTaskId))
                {
                    await DataValidator.AddErrorField("movementTaskId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.CancelMovementTask(movementTaskId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletemovementtask")]
        public async Task<ActionResult> DeleteMovementTask(string movementTaskId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTaskId))
                {
                    await DataValidator.AddErrorField("movementTaskId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTaskCore.DeleteMovementTask(movementTaskId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }
    }
}
