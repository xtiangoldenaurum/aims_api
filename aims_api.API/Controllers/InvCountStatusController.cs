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

    public class InvCountStatusController : ControllerBase
    {
        private IInvCountStatusCore InvCountStatusCore { get; set; }
        DataValidator DataValidator;

        public InvCountStatusController(IInvCountStatusCore invCountStatusCore)
        {
            InvCountStatusCore = invCountStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvcountstatuspg")]
        public async Task<ActionResult> GetInvCountStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvCountStatusCore.GetInvCountStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountstatuspgsrch")]
        public async Task<ActionResult> GetInvCountStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvCountStatusCore.GetInvCountStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountstatusbyid")]
        public async Task<ActionResult> GetInvCountStatusById(string invCountStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountStatusId))
                {
                    await DataValidator.AddErrorField("invCountStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountStatusCore.GetInvCountStatusById(invCountStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvcountstatus")]
        public async Task<ActionResult> CreateInvCountStatus(InvCountStatusModel invCountStatus)
        {
            try
            {
                if (invCountStatus == null)
                {
                    await DataValidator.AddErrorField("invCountStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountStatusCore.CreateInvCountStatus(invCountStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvcountstatus")]
        public async Task<ActionResult> UpdateInvCountStatus(InvCountStatusModel invCountStatus)
        {
            try
            {
                if (invCountStatus == null)
                {
                    await DataValidator.AddErrorField("invCountStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountStatusCore.UpdateInvCountStatus(invCountStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvcountstatus")]
        public async Task<ActionResult> DeleteInvCountStatus(string invCountStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountStatusId))
                {
                    await DataValidator.AddErrorField("invCountStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountStatusCore.DeleteInvCountStatus(invCountStatusId));
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
