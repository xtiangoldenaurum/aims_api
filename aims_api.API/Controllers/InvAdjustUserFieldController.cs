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
    public class InvAdjustUserFieldController : ControllerBase
    {
        private IInvAdjustUserFieldCore InvAdjustUserFieldCore { get; set; }
        DataValidator DataValidator;
        public InvAdjustUserFieldController(IInvAdjustUserFieldCore invAdjustUserFieldCore)
        {
            InvAdjustUserFieldCore = invAdjustUserFieldCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvadjustufields")]
        public async Task<ActionResult> GetInvAdjustUFields()
        {
            try
            {
                return Ok(await InvAdjustUserFieldCore.GetInvAdjustUFields());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustuserfieldbyid")]
        public async Task<ActionResult> GetInvAdjustUserFieldById(string invAdjustId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustUserFieldCore.GetInvAdjustUserFieldById(invAdjustId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvadjustufield")]
        public async Task<ActionResult> CreateInvAdjustUField(string fieldName, string createdBy)
        {
            try
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    await DataValidator.AddErrorField("fieldName");
                }
                if (string.IsNullOrEmpty(createdBy))
                {
                    await DataValidator.AddErrorField("createdBy");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustUserFieldCore.CreateInvAdjustUField(fieldName, createdBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvadjustufield")]
        public async Task<ActionResult> UpdateInvAdjustUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            try
            {
                if (string.IsNullOrEmpty(oldFieldName))
                {
                    await DataValidator.AddErrorField("oldFieldName");
                }
                if (string.IsNullOrEmpty(newFieldName))
                {
                    await DataValidator.AddErrorField("newFieldName");
                }
                if (string.IsNullOrEmpty(modifiedBy))
                {
                    await DataValidator.AddErrorField("modifiedBy");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustUserFieldCore.UpdateInvAdjustUField(oldFieldName, newFieldName, modifiedBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvadjustufield")]
        public async Task<ActionResult> DeleteInvAdjustUField(string fieldName, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    await DataValidator.AddErrorField("fieldName");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustUserFieldCore.DeleteInvAdjustUField(fieldName, userAccountId));
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
