using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class AccessRightController : ControllerBase
    {
        private IAccessRightCore AccessRightCore { get; set; }
        DataValidator DataValidator;

        public AccessRightController(IAccessRightCore accessRightCore)
        {
            AccessRightCore = accessRightCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getaccesrightsspecial")]
        public async Task<ActionResult> GetAccessRightsSpecial(string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await AccessRightCore.GetAccessRightsSpecial(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaccessrights")]
        public async Task<ActionResult> GetAccessRights()
        {
            try
            {
                return Ok(await AccessRightCore.GetAllAccessRight());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaccessrightspg")]
        public async Task<ActionResult> GetAccessRightsPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await AccessRightCore.GetAccessRightPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaccessrightpaged")]
        public async Task<ActionResult> GetAccessRightPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await AccessRightCore.GetAccessRightPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaccessrightspgsrch")]
        public async Task<ActionResult> GetAccessRightsPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await AccessRightCore.GetAccessRightPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaccessrightsrchpaged")]
        public async Task<ActionResult> GetAccessRightSrchPaged(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await AccessRightCore.GetAccessRightSrchPaged(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaccessrightbyid")]
        public async Task<ActionResult> GetAccessRightById(string accessRightId)
        {
            try
            {
                if (string.IsNullOrEmpty(accessRightId))
                {
                    await DataValidator.AddErrorField("accessRightId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AccessRightCore.GetAccessRightById(accessRightId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createaccessrightheader")]
        public async Task<ActionResult> CreateAccessRight(AccessRightModel accessRight)
        {
            try
            {
                if (accessRight == null)
                {
                    await DataValidator.AddErrorField("accessRight");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AccessRightCore.CreateAccessRightHeader(accessRight));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateaccessright")]
        public async Task<ActionResult> UpdateAccessRight(AccessRightHDModel accessRightHD)
        {
            try
            {
                if (accessRightHD == null)
                {
                    await DataValidator.AddErrorField("accessRightHD");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AccessRightCore.UpdateAccessRight(accessRightHD));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteaccessright")]
        public async Task<ActionResult> DeleteAccessRight(string accessRightId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(accessRightId))
                {
                    await DataValidator.AddErrorField("accessRightId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AccessRightCore.DeleteAccessRight(accessRightId, userAccountId));
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
