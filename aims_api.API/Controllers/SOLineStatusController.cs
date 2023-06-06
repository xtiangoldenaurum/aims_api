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

    public class SOLineStatusController : ControllerBase
    {
        private ISOLineStatusCore SOLineStatusCore { get; set; }
        DataValidator DataValidator;

        public SOLineStatusController(ISOLineStatusCore soLineStatusCore)
        {
            SOLineStatusCore = soLineStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getsolinestatuspg")]
        public async Task<ActionResult> GetSOLineStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await SOLineStatusCore.GetSOLineStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getsolinestatuspgsrch")]
        public async Task<ActionResult> GetSOLineStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await SOLineStatusCore.GetSOLineStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getsolinestatusbyid")]
        public async Task<ActionResult> GetSOLineStatusById(string soLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(soLineStatusId))
                {
                    await DataValidator.AddErrorField("soLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOLineStatusCore.GetSOLineStatusById(soLineStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createsolinestatus")]
        public async Task<ActionResult> CreateSOLineStatus(SOLineStatusModel soLineStatus)
        {
            try
            {
                if (soLineStatus == null)
                {
                    await DataValidator.AddErrorField("soLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOLineStatusCore.CreateSOLineStatus(soLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatesolinestatus")]
        public async Task<ActionResult> UpdateSOLineStatus(SOLineStatusModel soLineStatus)
        {
            try
            {
                if (soLineStatus == null)
                {
                    await DataValidator.AddErrorField("soLineStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOLineStatusCore.UpdateSOLineStatus(soLineStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletesolinestatus")]
        public async Task<ActionResult> DeleteSOLineStatus(string soLineStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(soLineStatusId))
                {
                    await DataValidator.AddErrorField("soLineStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOLineStatusCore.DeleteSOLineStatus(soLineStatusId));
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
