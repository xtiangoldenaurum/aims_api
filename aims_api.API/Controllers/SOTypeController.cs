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

    public class SOTypeController : ControllerBase
    {
        private ISOTypeCore SOTypeCore { get; set; }
        DataValidator DataValidator;

        public SOTypeController(ISOTypeCore soTypeCore)
        {
            SOTypeCore = soTypeCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getsotypepg")]
        public async Task<ActionResult> GetSOTypePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await SOTypeCore.GetSOTypePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getsotypepgsrch")]
        public async Task<ActionResult> GetSOTypePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await SOTypeCore.GetSOTypePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getsotypebyid")]
        public async Task<ActionResult> GetSOTypeById(string soTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(soTypeId))
                {
                    await DataValidator.AddErrorField("soTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOTypeCore.GetSOTypeById(soTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createsotype")]
        public async Task<ActionResult> CreateSOType(SOTypeModel soType)
        {
            try
            {
                if (soType == null)
                {
                    await DataValidator.AddErrorField("soType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOTypeCore.CreateSOType(soType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatesotype")]
        public async Task<ActionResult> UpdateSOType(SOTypeModel soType)
        {
            try
            {
                if (soType == null)
                {
                    await DataValidator.AddErrorField("soType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOTypeCore.UpdateSOType(soType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletesotype")]
        public async Task<ActionResult> DeleteSOType(string soTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(soTypeId))
                {
                    await DataValidator.AddErrorField("soTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOTypeCore.DeleteSOType(soTypeId));
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
