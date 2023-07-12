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
    public class InvAdjustController : ControllerBase
    {
        private IInvAdjustCore InvAdjustCore { get; set; }
        DataValidator DataValidator;
        public InvAdjustController(IInvAdjustCore invAdjustCore)
        {
            InvAdjustCore = invAdjustCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getinvadjustspecial")]
        public async Task<ActionResult> GetInvAdjustSpecial(InvAdjustFilteredMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvAdjustCore.GetInvAdjustSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw; //allows to create custom error.
            }
        }

        [HttpGet("getinvadjustforadjpaged")]
        public async Task<ActionResult> GetInvAdjustForAdjPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvAdjustCore.GetInvAdjustForAdjPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustpg")]
        public async Task<ActionResult> GetInvAdjustPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvAdjustCore.GetInvAdjustPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustpgsrch")]
        public async Task<ActionResult> GetInvAdjustPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvAdjustCore.GetInvAdjustPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustheaderbyid")]
        public async Task<ActionResult> GetInvAdjustHeaderById(string invAdjustId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.GetInvAdjustById(invAdjustId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustbyid")]
        public async Task<ActionResult> GetInvAdjustById(string invAdjustId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.GetInvAdjustByIdMod(invAdjustId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvadjustmod")]
        public async Task<ActionResult> CreateInvAdjustMod(InvAdjustModelMod invAdjust)
        {
            try
            {
                if (invAdjust == null)
                {
                    await DataValidator.AddErrorField("invAdjust");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.CreateInvAdjustMod(invAdjust));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvadjustmod")]
        public async Task<ActionResult> UpdateInvAdjustMod(InvAdjustModelMod invAdjust)
        {
            try
            {
                if (invAdjust == null)
                {
                    await DataValidator.AddErrorField("invAdjust");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.UpdateInvAdjustMod(invAdjust));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvadjust")]
        public async Task<ActionResult> DeleteInvAdjust(string invAdjustId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.DeleteInvAdjust(invAdjustId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("cancelinvadjust")]
        public async Task<ActionResult> CancelInvAdjust(string invAdjustId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.CancelInvAdjust(invAdjustId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }


        [HttpPost("forcecancelinvadjust")]
        public async Task<ActionResult> ForceCancelInvAdjust(string invAdjustId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.ForceCancelInvAdjust(invAdjustId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvadjustapproved")]
        public async Task<ActionResult> UpdateInvAdjustApprovedMod(InvAdjustModelMod invAdjust)
        {
            try
            {
                if (invAdjust == null)
                {
                    await DataValidator.AddErrorField("invAdjust");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustCore.UpdateInvAdjustApprovedMod(invAdjust));
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
