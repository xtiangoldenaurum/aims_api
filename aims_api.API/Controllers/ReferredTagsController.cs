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

    public class ReferredTagsController : ControllerBase
    {
        private IReferredTagsCore ReferredTagsCore { get; set; }
        DataValidator DataValidator;

        public ReferredTagsController(IReferredTagsCore referredTagsCore)
        {
            ReferredTagsCore = referredTagsCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getreferredtagspg")]
        public async Task<ActionResult> GetReferredTagsPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ReferredTagsCore.GetReferredTagsPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreferredtagspgsrch")]
        public async Task<ActionResult> GetReferredTagsPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ReferredTagsCore.GetReferredTagsPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreferredtagsbyid")]
        public async Task<ActionResult> GetReferredTagsById(int referredTagId)
        {
            try
            {
                if (referredTagId < 1)
                {
                    await DataValidator.AddErrorField("referredTagId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReferredTagsCore.GetReferredTagsById(referredTagId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createreferredtags")]
        public async Task<ActionResult> CreateReferredTags(ReferredTagsModel referredTags)
        {
            try
            {
                if (referredTags == null)
                {
                    await DataValidator.AddErrorField("referredTags");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReferredTagsCore.CreateReferredTags(referredTags));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatereferredtags")]
        public async Task<ActionResult> UpdateReferredTags(ReferredTagsModel referredTags)
        {
            try
            {
                if (referredTags == null)
                {
                    await DataValidator.AddErrorField("referredTags");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReferredTagsCore.UpdateReferredTags(referredTags));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletereferredtags")]
        public async Task<ActionResult> DeleteReferredTags(int referredTagId)
        {
            try
            {
                if (referredTagId < 1)
                {
                    await DataValidator.AddErrorField("referredTagId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReferredTagsCore.DeleteReferredTags(referredTagId));
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
