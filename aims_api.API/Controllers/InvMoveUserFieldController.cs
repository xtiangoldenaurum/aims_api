using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvMoveUserFieldController : ControllerBase
    {
        private IInvMoveUserFieldCore InvMoveUserFieldCore { get; set; }
        DataValidator DataValidator;
        public InvMoveUserFieldController(IInvMoveUserFieldCore invMoveUserFieldCore)
        {
            InvMoveUserFieldCore = invMoveUserFieldCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getpoufields")]
        public async Task<ActionResult> GetInvMoveUFields()
        {
            try
            {
                return Ok(await InvMoveUserFieldCore.GetInvMoveUFields());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpouserfieldbyid")]
        public async Task<ActionResult> GetInvMoveUserFieldById(string invMoveId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveUserFieldCore.GetInvMoveUserFieldById(invMoveId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createpoufield")]
        public async Task<ActionResult> CreateInvMoveUField(string fieldName, string createdBy)
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

                return Ok(await InvMoveUserFieldCore.CreateInvMoveUField(fieldName, createdBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatepoufield")]
        public async Task<ActionResult> UpdateInvMoveUField(string oldFieldName, string newFieldName, string modifiedBy)
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

                return Ok(await InvMoveUserFieldCore.UpdateInvMoveUField(oldFieldName, newFieldName, modifiedBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepoufield")]
        public async Task<ActionResult> DeleteInvMoveUField(string fieldName, string userAccountId)
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

                return Ok(await InvMoveUserFieldCore.DeleteInvMoveUField(fieldName, userAccountId));
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
