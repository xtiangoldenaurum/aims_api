using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvMoveController : ControllerBase
    {
        private IInvMoveCore InvMoveCore { get; set; }
        DataValidator DataValidator;
        public InvMoveController(IInvMoveCore invMoveCore)
        {
            InvMoveCore = invMoveCore;
            DataValidator = new DataValidator();
        }
        [HttpPost("getinvmovespecial")]
        public async Task<ActionResult> GetInvMoveSpecial(InvMoveFilteredMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvMoveCore.GetInvMoveSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw; //allows to create custom error.
            }
        }

        [HttpGet("getinvmoveformvpaged")]
        public async Task<ActionResult> GetInvMoveForMvPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvMoveCore.GetInvMoveForMvPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovepg")]
        public async Task<ActionResult> GetInvMovePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvMoveCore.GetInvMovePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovepgsrch")]
        public async Task<ActionResult> GetInvMovePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvMoveCore.GetInvMovePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmoveheaderbyid")]
        public async Task<ActionResult> GetInvMoveHeaderById(string invMoveId)
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

                return Ok(await InvMoveCore.GetInvMoveById(invMoveId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovebyid")]
        public async Task<ActionResult> GetInvMoveById(string invMoveId)
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

                return Ok(await InvMoveCore.GetInvMoveByIdMod(invMoveId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvmovemod")]
        public async Task<ActionResult> CreateInvMoveMod(InvMoveModelMod invMove)
        {
            try
            {
                if (invMove == null)
                {
                    await DataValidator.AddErrorField("invMove");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveCore.CreateInvMoveMod(invMove));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }
        [HttpPost("updateinvmovemod")]
        public async Task<ActionResult> UpdateInvMoveMod(InvMoveModelMod invMove)
        {
            try
            {
                if (invMove == null)
                {
                    await DataValidator.AddErrorField("invMove");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveCore.UpdateInvMoveMod(invMove));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvmove")]
        public async Task<ActionResult> DeleteInvMove(string invMoveId)
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

                return Ok(await InvMoveCore.DeleteInvMove(invMoveId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("cancelinvmove")]
        public async Task<ActionResult> CancelInvMove(string invMoveId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveCore.CancelInvMove(invMoveId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }


        [HttpPost("forcecancelinvmove")]
        public async Task<ActionResult> ForceCancelInvMove(string invMoveId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveCore.ForceCancelInvMove(invMoveId, userAccountId));
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
