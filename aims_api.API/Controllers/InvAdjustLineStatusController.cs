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

    public class InvAdjustLineStatusController : ControllerBase
    {
        private IInvAdjustLineStatusCore InvAdjustLineStatusCore { get; set; }
        DataValidator DataValidator;

        public InvAdjustLineStatusController(IInvAdjustLineStatusCore invAdjustLineStatusCore)
        {
            InvAdjustLineStatusCore = invAdjustLineStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvadjustlinestatuspg")]
        public async Task<ActionResult> GetInvAdjustLineStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvAdjustLineStatusCore.GetInvAdjustLineStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustlinestatuspgsrch")]
        public async Task<ActionResult> GetInvAdjustLineStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvAdjustLineStatusCore.GetInvAdjustLineStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustlinestatusbyid")]
        public async Task<ActionResult> GetInvAdjustLineStatusById(string invAdjustLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustLineStatusId))
                {
                    await DataValidator.AddErrorField("invAdjustLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustLineStatusCore.GetInvAdjustLineStatusById(invAdjustLineStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvadjustlinestatus")]
        public async Task<ActionResult> CreateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus)
        {
            try
            {
                if (invAdjustLineStatus == null)
                {
                    await DataValidator.AddErrorField("invAdjustLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustLineStatusCore.CreateInvAdjustLineStatus(invAdjustLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvadjustlinestatus")]
        public async Task<ActionResult> UpdateInvAdjustLineStatus(InvAdjustLineStatusModel invAdjustLineStatus)
        {
            try
            {
                if (invAdjustLineStatus == null)
                {
                    await DataValidator.AddErrorField("invAdjustLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustLineStatusCore.UpdateInvAdjustLineStatus(invAdjustLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvadjustlinestatus")]
        public async Task<ActionResult> DeleteInvAdjustLineStatus(string invAdjustLineStatusId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustLineStatusId))
                {
                    await DataValidator.AddErrorField("invAdjustLineStatusId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustLineStatusCore.DeleteInvAdjustLineStatus(invAdjustLineStatusId, userAccountId));
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
