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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class AuditTrailController : ControllerBase
    {
        private IAuditTrailCore AuditTrailCore { get; set; }
        DataValidator DataValidator;

        public AuditTrailController(IAuditTrailCore auditTrailCore)
        {
            AuditTrailCore = auditTrailCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getaudittrailspecial")]
        public async Task<ActionResult> GetAuditTrailSpecial(AuditTrailFilterMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await AuditTrailCore.GetAuditTrailSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaudittrailpg")]
        public async Task<ActionResult> GetAuditTrailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await AuditTrailCore.GetAuditTrailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaudittrailpaged")]
        public async Task<ActionResult> GetAuditTrailPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await AuditTrailCore.GetAuditTrailPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaudittrailpgsrch")]
        public async Task<ActionResult> GetAuditTrailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await AuditTrailCore.GetAuditTrailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaudittrailsrchpaged")]
        public async Task<ActionResult> GetAuditTrailSrchPaged(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await AuditTrailCore.GetAuditTrailSrchPaged(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getaudittrailbyid")]
        public async Task<ActionResult> GetAuditTrailById(int auditId)
        {
            try
            {
                if (auditId < 1)
                {
                    await DataValidator.AddErrorField("auditId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AuditTrailCore.GetAuditTrailById(auditId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getaudittrailpgfiltered")]
        public async Task<ActionResult> GetAuditTrailPgFiltered(AuditTrailFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.RecordId) && 
                        string.IsNullOrEmpty(filter.UserAccountId) && 
                        string.IsNullOrEmpty(filter.TransactionTypeId) && 
                        filter.AuditDateFrom == null && 
                        filter.AuditDateTo == null)
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AuditTrailCore.GetAuditTrailPgFiltered(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("getaudittrailfltrpaged")]
        public async Task<ActionResult> GetAuditTrailFltrPaged(AuditTrailFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.RecordId) &&
                        string.IsNullOrEmpty(filter.UserAccountId) &&
                        string.IsNullOrEmpty(filter.TransactionTypeId) &&
                        filter.AuditDateFrom == null &&
                        filter.AuditDateTo == null)
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AuditTrailCore.GetAuditTrailFltrPaged(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createaudittrail")]
        public async Task<ActionResult> CreatetblName(AuditTrailModel auditTrail)
        {
            try
            {
                if (auditTrail == null)
                {
                    await DataValidator.AddErrorField("auditTrail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AuditTrailCore.CreateAuditTrail(auditTrail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatearea")]
        public async Task<ActionResult> UpdateAuditTrail(AuditTrailModel auditTrail)
        {
            try
            {
                if (auditTrail == null)
                {
                    await DataValidator.AddErrorField("auditTrail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AuditTrailCore.UpdateAuditTrail(auditTrail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteauditrail")]
        public async Task<ActionResult> DeleteAuditTrail(int auditId)
        {
            try
            {
                if (auditId < 1)
                {
                    await DataValidator.AddErrorField("auditId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await AuditTrailCore.DeleteAuditTrail(auditId));
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
