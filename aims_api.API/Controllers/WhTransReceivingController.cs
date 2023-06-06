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

    public class WhTransReceivingController : ControllerBase
    {
        private IWhTransReceivingCore WhTransReceivingCore { get; set; }
        DataValidator DataValidator;

        public WhTransReceivingController(IWhTransReceivingCore whTransReceivingCore)
        {
            WhTransReceivingCore = whTransReceivingCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getreceivesbywhtransid")]
        public async Task<ActionResult> GetReceivesByWhTransId(string whTransId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransId))
                {
                    await DataValidator.AddErrorField("whTransId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransReceivingCore.GetReceivesByWhTransId(whTransId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getcancelablercvsbyid")]
        public async Task<ActionResult> GetCancelableRcvsById(string whTranLineId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(whTranLineId))
                {
                    await DataValidator.AddErrorField("whTranLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransReceivingCore.GetCancelableRcvsById(whTranLineId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getcancelablercvsbyretid")]
        public async Task<ActionResult> GetCancelableRcvsByWhTransId(string whTransId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransId))
                {
                    await DataValidator.AddErrorField("whTransId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransReceivingCore.GetCancelableRcvsByWhTransId(whTransId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreceivingpgsrch")]
        public async Task<ActionResult> GetReceivingPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await WhTransReceivingCore.GetReceivingPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreceivingbyid")]
        public async Task<ActionResult> GetReceivingById(string receivingId)
        {
            try
            {
                if (string.IsNullOrEmpty(receivingId))
                {
                    await DataValidator.AddErrorField("receivingId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransReceivingCore.GetReceivingById(receivingId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("receiving")]
        public async Task<ActionResult> Receiving(WhTransReceivingModelMod data)
        {
            try
            {
                return Ok(await WhTransReceivingCore.Receiving(data));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("cancelreceived")]
        public async Task<ActionResult> CancelReceived(string receivingId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(receivingId))
                {
                    await DataValidator.AddErrorField("receivingId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransReceivingCore.CancelReceived(receivingId, userAccountId));
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
