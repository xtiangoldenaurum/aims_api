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

    public class RunningBalanceController : ControllerBase
    {
        private IRunningBalanceCore RunningBalanceCore { get; set; }
        DataValidator DataValidator;

        public RunningBalanceController(IRunningBalanceCore runningBalanceCore)
        {
            RunningBalanceCore = runningBalanceCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getrunningbalancepg")]
        public async Task<ActionResult> GetRunningBalancePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await RunningBalanceCore.GetRunningBalancePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getrunningbalancepgsrch")]
        public async Task<ActionResult> GetRunningBalancePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await RunningBalanceCore.GetRunningBalancePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getrunningbalancebyid")]
        public async Task<ActionResult> GetRunningBalanceById(string movementTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTypeId))
                {
                    await DataValidator.AddErrorField("movementTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await RunningBalanceCore.GetRunningBalanceById(movementTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createrunningbalance")]
        public async Task<ActionResult> CreateRunningBalance(RunningBalanceModel runningBalance)
        {
            try
            {
                if (runningBalance == null)
                {
                    await DataValidator.AddErrorField("runningBalance");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await RunningBalanceCore.CreateRunningBalance(runningBalance));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updaterunningbalance")]
        public async Task<ActionResult> UpdateRunningBalance(RunningBalanceModel runningBalance)
        {
            try
            {
                if (runningBalance == null)
                {
                    await DataValidator.AddErrorField("runningBalance");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await RunningBalanceCore.UpdateRunningBalance(runningBalance));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleterunningbalance")]
        public async Task<ActionResult> DeleteRunningBalance(string movementTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTypeId))
                {
                    await DataValidator.AddErrorField("movementTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await RunningBalanceCore.DeleteRunningBalance(movementTypeId));
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
