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

    public class UtilitiesController : ControllerBase
    {
        private IUtilitiesCore UtilitiesCore { get; set; }
        DataValidator DataValidator;

        public UtilitiesController(IUtilitiesCore utilitiesCore)
        {
            UtilitiesCore = utilitiesCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("definetrantypbydocid")]
        public async Task<ActionResult> DefineTranTypeByDocId(string documentId)
        {
            try
            {
                if (string.IsNullOrEmpty(documentId))
                {
                    await DataValidator.AddErrorField("documentId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UtilitiesCore.DefineTranTypeByDocId(documentId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("definetrantypbydoclineid")]
        public async Task<ActionResult> DefineTranTypeByDocLineId(string docLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(docLineId))
                {
                    await DataValidator.AddErrorField("docLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UtilitiesCore.DefineTranTypeByDocLineId(docLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("gettenantsettings")]
        public async Task<ActionResult> GetTenantSettings()
        {
            try
            {
                return Ok(await UtilitiesCore.GetTenantSettings());
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
