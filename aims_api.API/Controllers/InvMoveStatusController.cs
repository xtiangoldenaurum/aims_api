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

    public class InvMoveStatusController : ControllerBase
    {
        private IInvMoveStatusCore InvMoveStatusCore { get; set; }
        DataValidator DataValidator;

        public InvMoveStatusController(IInvMoveStatusCore invMoveStatusCore)
        {
            InvMoveStatusCore = invMoveStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvmovestatuspg")]
        public async Task<ActionResult> GetInvMoveStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvMoveStatusCore.GetInvMoveStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovestatuspgsrch")]
        public async Task<ActionResult> GetInvMoveStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvMoveStatusCore.GetInvMoveStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvmovestatusbyid")]
        public async Task<ActionResult> GetInvMoveStatusById(string invMoveStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveStatusId))
                {
                    await DataValidator.AddErrorField("invMoveStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveStatusCore.GetInvMoveStatusById(invMoveStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvmovestatus")]
        public async Task<ActionResult> CreateInvMoveStatus(InvMoveStatusModel invMoveStatus)
        {
            try
            {
                if (invMoveStatus == null)
                {
                    await DataValidator.AddErrorField("invMoveStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveStatusCore.CreateInvMoveStatus(invMoveStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvmovestatus")]
        public async Task<ActionResult> UpdateInvMoveStatus(InvMoveStatusModel invMoveStatus)
        {
            try
            {
                if (invMoveStatus == null)
                {
                    await DataValidator.AddErrorField("invMoveStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveStatusCore.UpdateInvMoveStatus(invMoveStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvmovestatus")]
        public async Task<ActionResult> DeleteInvMoveStatus(string invMoveStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveStatusId))
                {
                    await DataValidator.AddErrorField("invMoveStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveStatusCore.DeleteInvMoveStatus(invMoveStatusId));
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
