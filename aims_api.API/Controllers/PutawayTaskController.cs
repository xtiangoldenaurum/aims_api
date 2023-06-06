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

    public class PutawayTaskController : ControllerBase
    {
        private IPutawayTaskCore PutawayTaskCore { get; set; }
        DataValidator DataValidator;

        public PutawayTaskController(IPutawayTaskCore putawayTaskCore)
        {
            PutawayTaskCore = putawayTaskCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getputawaytaskpg")]
        public async Task<ActionResult> GetPutawayTaskPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await PutawayTaskCore.GetPutawayTaskPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getputawaytaskpgsrch")]
        public async Task<ActionResult> GetPutawayTaskPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await PutawayTaskCore.GetPutawayTaskPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getputawaytaskbyid")]
        public async Task<ActionResult> GetPutawayTaskById(string putawayTaskId)
        {
            try
            {
                if (string.IsNullOrEmpty(putawayTaskId))
                {
                    await DataValidator.AddErrorField("putawayTaskId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.GetPutawayTaskById(putawayTaskId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createputawaytask")]
        public async Task<ActionResult> CreatePutawayTask(PutawayTaskModel putawayTask)
        {
            try
            {
                if (putawayTask == null)
                {
                    await DataValidator.AddErrorField("putawayTask");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.CreatePutawayTask(putawayTask));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateputawaytask")]
        public async Task<ActionResult> UpdatePutawayTask(PutawayTaskModel putawayTask)
        {
            try
            {
                if (putawayTask == null)
                {
                    await DataValidator.AddErrorField("putawayTask");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.UpdatePutawayTask(putawayTask));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteputawaytask")]
        public async Task<ActionResult> DeletePutawayTask(string putawayTaskId)
        {
            try
            {
                if (string.IsNullOrEmpty(putawayTaskId))
                {
                    await DataValidator.AddErrorField("putawayTaskId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.DeletePutawayTask(putawayTaskId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("putawayqrytiddetails")]
        public async Task<ActionResult> PutawayQryTIDDetails(string trackId, string userAccountId)
        {
            try
            {
                if (trackId == null)
                {
                    await DataValidator.AddErrorField("trackId");
                }
                if (userAccountId == null)
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.PutawayQryTIDDetails(trackId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("commitputaway")]
        public async Task<ActionResult> CommitPutaway(PutawayTaskProcModel data)
        {
            try
            {
                if (data == null)
                {
                    await DataValidator.AddErrorField("data");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.CommitPutaway(data));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("querylpnputaway")]
        public async Task<ActionResult> QueryLPNPUtaway(string palletId)
        {
            try
            {
                if (palletId == null)
                {
                    await DataValidator.AddErrorField("palletId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.QueryLPNPUtaway(palletId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("proceedpalletputaway")]
        public async Task<ActionResult> ProceedPalletPutaway(CommitPalletPutawayModel data)
        {
            try
            {
                if (data == null)
                {
                    await DataValidator.AddErrorField("data");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PutawayTaskCore.ProceedPalletPutaway(data));
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
