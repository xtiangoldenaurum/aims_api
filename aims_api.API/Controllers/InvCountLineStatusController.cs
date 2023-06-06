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

    public class InvCountLineStatusController : ControllerBase
    {
        private IInvCountLineStatusCore InvCountLineStatusCore { get; set; }
        DataValidator DataValidator;

        public InvCountLineStatusController(IInvCountLineStatusCore invCountLineStatusCore)
        {
            InvCountLineStatusCore = invCountLineStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvcountlinestatuspg")]
        public async Task<ActionResult> GetInvCountLineStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvCountLineStatusCore.GetInvCountLineStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountlinestatuspgsrch")]
        public async Task<ActionResult> GetInvCountLineStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvCountLineStatusCore.GetInvCountLineStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountlinestatusbyid")]
        public async Task<ActionResult> GetInvCountLineStatusById(string invCountLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountLineStatusId))
                {
                    await DataValidator.AddErrorField("invCountLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountLineStatusCore.GetInvCountLineStatusById(invCountLineStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvcountlinestatus")]
        public async Task<ActionResult> CreateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus)
        {
            try
            {
                if (invCountLineStatus == null)
                {
                    await DataValidator.AddErrorField("invCountLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountLineStatusCore.CreateInvCountLineStatus(invCountLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvcountlinestatus")]
        public async Task<ActionResult> UpdateInvCountLineStatus(InvCountLineStatusModel invCountLineStatus)
        {
            try
            {
                if (invCountLineStatus == null)
                {
                    await DataValidator.AddErrorField("invCountLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountLineStatusCore.UpdateInvCountLineStatus(invCountLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvcountlinestatus")]
        public async Task<ActionResult> DeleteInvCountLineStatus(string invCountLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountLineStatusId))
                {
                    await DataValidator.AddErrorField("invCountLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountLineStatusCore.DeleteInvCountLineStatus(invCountLineStatusId));
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
