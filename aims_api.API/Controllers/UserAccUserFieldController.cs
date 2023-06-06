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

    public class UserAccUserFieldController : ControllerBase
    {
        private IUserAccUserFieldCore UserAccUFieldCore { get; set; }
        DataValidator DataValidator;

        public UserAccUserFieldController(IUserAccUserFieldCore userAccUFieldCore)
        {
            UserAccUFieldCore = userAccUFieldCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getaccuserfieldbyid")]
        public async Task<ActionResult> GetAccUserFieldById(string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UserAccUFieldCore.GetAccUserFieldById(userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuseraccountufields")]
        public async Task<ActionResult> GetUserAccountUFields()
        {
            try
            {
                return Ok(await UserAccUFieldCore.GetUserAccountUFields());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createuseraccufield")]
        public async Task<ActionResult> CreateUserAccUField(string fieldName, string createdBy)
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

                return Ok(await UserAccUFieldCore.CreateUserAccUField(fieldName, createdBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateuseraccufield")]
        public async Task<ActionResult> UpdateSOUField(string oldFieldName, string newFieldName, string modifiedBy)
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

                return Ok(await UserAccUFieldCore.UpdateUserAccUField(oldFieldName, newFieldName, modifiedBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteuseraccufield")]
        public async Task<ActionResult> DeleteUserAccUField(string fieldName, string userAccountId)
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

                return Ok(await UserAccUFieldCore.DeleteUserAccUField(fieldName, userAccountId));
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
