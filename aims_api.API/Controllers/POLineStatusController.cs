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

    public class POLineStatusController : ControllerBase
    {
        private IPOLineStatusCore POLineStatusCore { get; set; }
        DataValidator DataValidator;

        public POLineStatusController(IPOLineStatusCore poLineStatusCore)
        {
            POLineStatusCore = poLineStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getpolinestatuspg")]
        public async Task<ActionResult> GetPOLineStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await POLineStatusCore.GetPOLineStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpolinestatuspgsrch")]
        public async Task<ActionResult> GetPOLineStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await POLineStatusCore.GetPOLineStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpolinestatusbyid")]
        public async Task<ActionResult> GetPOLineStatusById(string poLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(poLineStatusId))
                {
                    await DataValidator.AddErrorField("poLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POLineStatusCore.GetPOLineStatusById(poLineStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createpolinestatus")]
        public async Task<ActionResult> CreatePOLineStatus(POLineStatusModel poLineStatus)
        {
            try
            {
                if (poLineStatus == null)
                {
                    await DataValidator.AddErrorField("poLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POLineStatusCore.CreatePOLineStatus(poLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatepolinestatus")]
        public async Task<ActionResult> UpdatePOLineStatus(POLineStatusModel poLineStatus)
        {
            try
            {
                if (poLineStatus == null)
                {
                    await DataValidator.AddErrorField("poLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POLineStatusCore.UpdatePOLineStatus(poLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepolinestatus")]
        public async Task<ActionResult> DeletePOLineStatus(string poLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(poLineStatusId))
                {
                    await DataValidator.AddErrorField("poLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POLineStatusCore.DeletePOLineStatus(poLineStatusId));
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
