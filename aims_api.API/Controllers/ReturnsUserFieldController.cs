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

    public class ReturnsUserFieldController : ControllerBase
    {
        private IReturnsUserFieldCore ReturnsUserFieldCore { get; set; }
        DataValidator DataValidator;

        public ReturnsUserFieldController(IReturnsUserFieldCore returnsUserFieldCore)
        {
            ReturnsUserFieldCore = returnsUserFieldCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getreturnsufields")]
        public async Task<ActionResult> GetReturnsUFields()
        {
            try
            {
                return Ok(await ReturnsUserFieldCore.GetReturnsUFields());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsuserfieldbyid")]
        public async Task<ActionResult> GetReturnsUserFieldById(string returnsId)
        {
            try
            {
                if (string.IsNullOrEmpty(returnsId))
                {
                    await DataValidator.AddErrorField("returnsId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsUserFieldCore.GetReturnsUserFieldById(returnsId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createreturnsufield")]
        public async Task<ActionResult> CreateReturnsUField(string fieldName, string createdBy)
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

                return Ok(await ReturnsUserFieldCore.CreateReturnsUField(fieldName, createdBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatereturnsufield")]
        public async Task<ActionResult> UpdateReturnsUField(string oldFieldName, string newFieldName, string modifiedBy)
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

                return Ok(await ReturnsUserFieldCore.UpdateReturnsUField(oldFieldName, newFieldName, modifiedBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletereturnsufield")]
        public async Task<ActionResult> DeleteReturnsUField(string fieldName, string userAccountId)
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

                return Ok(await ReturnsUserFieldCore.DeleteReturnsUField(fieldName, userAccountId));
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
