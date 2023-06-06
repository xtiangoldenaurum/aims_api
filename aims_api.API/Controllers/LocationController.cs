using aims_api.Cores.Implementation;
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

    public class LocationController : ControllerBase
    {
        private ILocationCore LocationCore { get; set; }
        DataValidator DataValidator;

        public LocationController(ILocationCore locationCore)
        {
            LocationCore = locationCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getlocationspecial")]
        public async Task<ActionResult> GetlocationSpecial(LocationFilterMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LocationCore.GetLocationSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationpg")]
        public async Task<ActionResult> GetLocationPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LocationCore.GetLocationPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationpaged")]
        public async Task<ActionResult> GetLocationPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LocationCore.GetLocationPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinbstatinglocationpaged")]
        public async Task<ActionResult> GetInbStatingLocationPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LocationCore.GetInbStatingLocationPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getoutstaginglocationpaged")]
        public async Task<ActionResult> GetOutStatingLocationPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LocationCore.GetOutStatingLocationPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationpgsrch")]
        public async Task<ActionResult> GetLocationPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await LocationCore.GetLocationPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationsrchpaged")]
        public async Task<ActionResult> GetLocationSrchPaged(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await LocationCore.GetLocationPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlocationbyid")]
        public async Task<ActionResult> GetLocationById(string locationId)
        {
            try
            {
                if (string.IsNullOrEmpty(locationId))
                {
                    await DataValidator.AddErrorField("locationId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.GetLocationById(locationId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getdistinctaisle")]
        public async Task<ActionResult> GetDIstinctAisle()
        {
            try
            {
                return Ok(await LocationCore.GetDIstinctAisle());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getdistinctbay")]
        public async Task<ActionResult> GetDIstinctBay()
        {
            try
            {
                return Ok(await LocationCore.GetDIstinctBay());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getlocationpgfiltered")]
        public async Task<ActionResult> GetLocationPgFiltered(LocationFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");

                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.LocationTypeId) &&
                        string.IsNullOrEmpty(filter.LocationGroupId) &&
                        string.IsNullOrEmpty(filter.AreaId) &&
                        filter.Inactive == null &&
                        string.IsNullOrEmpty(filter.AisleCode) &&
                        string.IsNullOrEmpty(filter.BayCode))
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.GetLocationPgFiltered(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getlocationfltrpaged")]
        public async Task<ActionResult> GetLocationFltrPaged(LocationFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");

                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.LocationTypeId) &&
                        string.IsNullOrEmpty(filter.LocationGroupId) &&
                        string.IsNullOrEmpty(filter.AreaId) &&
                        filter.Inactive == null &&
                        string.IsNullOrEmpty(filter.AisleCode) &&
                        string.IsNullOrEmpty(filter.BayCode))
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.GetLocationFltrPaged(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createlocation")]
        public async Task<ActionResult> CreateLocation(LocationModel location)
        {
            try
            {
                if (location == null)
                {
                    await DataValidator.AddErrorField("location");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.CreateLocation(location));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatelocation")]
        public async Task<ActionResult> UpdateLocation(LocationModel location)
        {
            try
            {
                if (location == null)
                {
                    await DataValidator.AddErrorField("location");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.UpdateLocation(location));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletelocation")]
        public async Task<ActionResult> DeleteLocation(string locationId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(locationId))
                {
                    await DataValidator.AddErrorField("locationId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.DeleteLocation(locationId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("definetargetlocation")]
        public async Task<ActionResult> DefineTargetLocation(string locVCode)
        {
            try
            {
                if (string.IsNullOrEmpty(locVCode))
                {
                    await DataValidator.AddErrorField("locVCode");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.DefineTargetLocation(locVCode));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("definelpnputawayloc")]
        public async Task<ActionResult> DefineLPNPutawayLoc(string lpnId, string locVCode)
        {
            try
            {
                if (string.IsNullOrEmpty(lpnId))
                {
                    await DataValidator.AddErrorField("lpnId");
                }
                if (string.IsNullOrEmpty(locVCode))
                {
                    await DataValidator.AddErrorField("locVCode");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LocationCore.DefineLPNPutawayLoc(lpnId, locVCode));
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
