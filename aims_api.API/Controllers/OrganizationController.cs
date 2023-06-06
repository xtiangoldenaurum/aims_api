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

    public class OrganizationController : ControllerBase
    {
        private IOrganizationCore OrganizationCore { get; set; }
        DataValidator DataValidator;

        public OrganizationController(IOrganizationCore organizationCore)
        {
            OrganizationCore = organizationCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getorgspecial")]
        public async Task<ActionResult> GetOrgSpecial(OrganizationFilterMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await OrganizationCore.GetOrgSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorganizationpg")]
        public async Task<ActionResult> GetOrganizationPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await OrganizationCore.GetOrganizationPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorganizationpgsrch")]
        public async Task<ActionResult> GetOrganizationPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await OrganizationCore.GetOrganizationPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorgpaged")]
        public async Task<ActionResult> GetOrgPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await OrganizationCore.GetOrgPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorgsearchpaged")]
        public async Task<ActionResult> GetOrgSearchPaged(string searchKey, int pageNum = 1, int pageItem = 100)
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
                return Ok(await OrganizationCore.GetOrgSearchPaged(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorganizationbyid")]
        public async Task<ActionResult> GetOrganizationById(string organizationId)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationId))
                {
                    await DataValidator.AddErrorField("organizationId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.GetOrganizationById(organizationId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getorgpgfiltered")]
        public async Task<ActionResult> GetOrgPgFiltered(OrganizationFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.OrganizationTypeId) && 
                        filter.Inactive == null)
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.GetOrgPgFiltered(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getorgpgbyorgtypid")]
        public async Task<ActionResult> GetOrgPgByOrgTypId(string organizationTypeId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationTypeId))
                {
                    await DataValidator.AddErrorField("organizationTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.GetOrgPgByOrgTypId(organizationTypeId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getorgfilteredpaged")]
        public async Task<ActionResult> GetOrgFilteredPaged(OrganizationFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.OrganizationTypeId) &&
                        filter.Inactive == null)
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.GetOrgFilteredPaged(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createorganization")]
        public async Task<ActionResult> CreateOrganization(OrganizationModel organization)
        {
            try
            {
                if (organization == null)
                {
                    await DataValidator.AddErrorField("organization");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.CreateOrganization(organization));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateorganization")]
        public async Task<ActionResult> UpdateOrganization(OrganizationModel organization)
        {
            try
            {
                if (organization == null)
                {
                    await DataValidator.AddErrorField("organization");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.UpdateOrganization(organization));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteorganization")]
        public async Task<ActionResult> DeleteOrganization(string organizationId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationId))
                {
                    await DataValidator.AddErrorField("organizationId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await OrganizationCore.DeleteOrganization(organizationId, userAccountId));
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
