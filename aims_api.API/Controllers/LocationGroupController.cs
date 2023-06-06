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

    public class LocationGroupController : ControllerBase
    {
        private ILocationGroupCore LocationGroupCore { get; set; }
        DataValidator DataValidator;

        public LocationGroupController(ILocationGroupCore locationGroupCore)
        {
            LocationGroupCore = locationGroupCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getlocationgrouppg")]
        public async Task<ActionResult> GetLocationGroupPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LocationGroupCore.GetLocationGroupPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationgrouppgsrch")]
        public async Task<ActionResult> GetLocationGroupPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await LocationGroupCore.GetLocationGroupPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationgroupbyid")]
        public async Task<ActionResult> GetLocationGroupById(string locationGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(locationGroupId))
                {
                    await DataValidator.AddErrorField("locationGroupId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationGroupCore.GetLocationGroupById(locationGroupId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createlocationgroup")]
        public async Task<ActionResult> CreateLocationGroup(LocationGroupModel locationGroup)
        {
            try
            {
                if (locationGroup == null)
                {
                    await DataValidator.AddErrorField("locationGroup");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationGroupCore.CreateLocationGroup(locationGroup));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatelocationgroup")]
        public async Task<ActionResult> UpdateLocationGroup(LocationGroupModel locationGroup)
        {
            try
            {
                if (locationGroup == null)
                {
                    await DataValidator.AddErrorField("locationGroup");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationGroupCore.UpdateLocationGroup(locationGroup));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletelocationgroup")]
        public async Task<ActionResult> DeleteLocationGroup(string locationGroupId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(locationGroupId))
                {
                    await DataValidator.AddErrorField("locationGroupId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationGroupCore.DeleteLocationGroup(locationGroupId, userAccountId));
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
