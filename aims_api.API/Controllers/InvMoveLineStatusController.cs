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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class InvMoveLineStatusController : ControllerBase
    {
        private IInvMoveLineStatusCore InvMoveLineStatusCore { get; set; }
        DataValidator DataValidator;

        public InvMoveLineStatusController(IInvMoveLineStatusCore invMoveLineStatusCore)
        {
            InvMoveLineStatusCore = invMoveLineStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvmovelinestatuspg")]
        public async Task<ActionResult> GetInvMoveLineStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvMoveLineStatusCore.GetInvMoveLineStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovelinestatuspgsrch")]
        public async Task<ActionResult> GetInvMoveLineStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvMoveLineStatusCore.GetInvMoveLineStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovelinestatusbyid")]
        public async Task<ActionResult> GetInvMoveLineStatusById(string invMoveLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveLineStatusId))
                {
                    await DataValidator.AddErrorField("invMoveLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveLineStatusCore.GetInvMoveLineStatusById(invMoveLineStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvmovelinestatus")]
        public async Task<ActionResult> CreateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus)
        {
            try
            {
                if (invMoveLineStatus == null)
                {
                    await DataValidator.AddErrorField("invMoveLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveLineStatusCore.CreateInvMoveLineStatus(invMoveLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatearea")]
        public async Task<ActionResult> UpdateInvMoveLineStatus(InvMoveLineStatusModel invMoveLineStatus)
        {
            try
            {
                if (invMoveLineStatus == null)
                {
                    await DataValidator.AddErrorField("invMoveLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveLineStatusCore.UpdateInvMoveLineStatus(invMoveLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvmovelinestatus")]
        public async Task<ActionResult> DeleteInvMoveLineStatus(string invMoveLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveLineStatusId))
                {
                    await DataValidator.AddErrorField("invMoveLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveLineStatusCore.DeleteInvMoveLineStatus(invMoveLineStatusId));
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
