using aims_api.Cores.Interface;
using aims_api.Enums;
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

    public class AccessRightDetailController : ControllerBase
    {
        private IAccessRightDetailCore AccessRightDetailCore { get; set; }

        public AccessRightDetailController(IAccessRightDetailCore accessRightDetailCore)
        {
            AccessRightDetailCore = accessRightDetailCore;
        }

        [HttpGet("getaccessrightdetails")]
        public async Task<ActionResult> GetAccessRightDetails()
        {
            try
            {
                return Ok(await AccessRightDetailCore.GetAllAccessRightDetail());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
            
        }

        [HttpGet("getaccessrightdetailsbyid")]
        public async Task<ActionResult> GetAccessRightDetailsById(string accesRightId)
        {
            try
            {
                return Ok(await AccessRightDetailCore.GetAccessRightDetailById(accesRightId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccessdetails")]
        public async Task<ActionResult> GetUserAccessDetails(string accesRightId)
        {
            try
            {
                return Ok(await AccessRightDetailCore.GetUserAccessDetails(accesRightId));
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
