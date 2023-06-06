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

    public class InvAdjustStatusController : ControllerBase
    {
        private IInvAdjustStatusCore InvAdjustStatusCore { get; set; }
        DataValidator DataValidator;

        public InvAdjustStatusController(IInvAdjustStatusCore invAdjustStatusCore)
        {
            InvAdjustStatusCore = invAdjustStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvadjuststatuspg")]
        public async Task<ActionResult> GetInvAdjustStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvAdjustStatusCore.GetInvAdjustStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjuststatuspgsrch")]
        public async Task<ActionResult> GetInvAdjustStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvAdjustStatusCore.GetInvAdjustStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjuststatusbyid")]
        public async Task<ActionResult> GetInvAdjustStatusById(string invAdjustStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustStatusId))
                {
                    await DataValidator.AddErrorField("invAdjustStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustStatusCore.GetInvAdjustStatusById(invAdjustStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvadjuststatus")]
        public async Task<ActionResult> CreateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus)
        {
            try
            {
                if (invAdjustStatus == null)
                {
                    await DataValidator.AddErrorField("invAdjustStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustStatusCore.CreateInvAdjustStatus(invAdjustStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvadjuststatus")]
        public async Task<ActionResult> UpdateInvAdjustStatus(InvAdjustStatusModel invAdjustStatus)
        {
            try
            {
                if (invAdjustStatus == null)
                {
                    await DataValidator.AddErrorField("invAdjustStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustStatusCore.UpdateInvAdjustStatus(invAdjustStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvadjuststatus")]
        public async Task<ActionResult> DeleteInvAdjustStatus(string invAdjustStatusId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustStatusId))
                {
                    await DataValidator.AddErrorField("invAdjustStatusId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustStatusCore.DeleteInvAdjustStatus(invAdjustStatusId, userAccountId));
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
