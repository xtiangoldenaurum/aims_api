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

    public class OrganizationTypeController : ControllerBase
    {
        private IOrganizationTypeCore OrganizationTypeCore { get; set; }
        DataValidator DataValidator;

        public OrganizationTypeController(IOrganizationTypeCore organizationTypeCore)
        {
            OrganizationTypeCore = organizationTypeCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getorganizationtypepg")]
        public async Task<ActionResult> GetOrganizationTypePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await OrganizationTypeCore.GetOrganizationTypePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorganizationtypepgsrch")]
        public async Task<ActionResult> GetOrganizationTypePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await OrganizationTypeCore.GetOrganizationTypePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorganizationtypebyid")]
        public async Task<ActionResult> GetOrganizationTypeById(string organizationTypeID)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationTypeID))
                {
                    await DataValidator.AddErrorField("organizationTypeID");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationTypeCore.GetOrganizationTypeById(organizationTypeID));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createorganizationtype")]
        public async Task<ActionResult> CreateOrganizationType(OrganizationTypeModel organizationType)
        {
            try
            {
                if (organizationType == null)
                {
                    await DataValidator.AddErrorField("organizationType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationTypeCore.CreateOrganizationType(organizationType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateorganizationtype")]
        public async Task<ActionResult> UpdateOrganizationType(OrganizationTypeModel organizationType)
        {
            try
            {
                if (organizationType == null)
                {
                    await DataValidator.AddErrorField("organizationType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationTypeCore.UpdateOrganizationType(organizationType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteorganizationtype")]
        public async Task<ActionResult> DeleteOrganizationType(string organizationTypeID)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationTypeID))
                {
                    await DataValidator.AddErrorField("organizationTypeID");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationTypeCore.DeleteOrganizationType(organizationTypeID));
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
