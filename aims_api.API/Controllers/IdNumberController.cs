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

    public class IdNumberController : ControllerBase
    {
        private IIdNumberCore IdNumberCore { get; set; }
        DataValidator DataValidator;

        public IdNumberController(IIdNumberCore idNumberCore)
        {
            IdNumberCore = idNumberCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getidnumberpg")]
        public async Task<ActionResult> GetIdNumberPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await IdNumberCore.GetIdNumberPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getidnumberpgsrch")]
        public async Task<ActionResult> GetIdNumberPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await IdNumberCore.GetIdNumberPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getidnumberbyid")]
        public async Task<ActionResult> GetIdNumberById(string transactionTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(transactionTypeId))
                {
                    await DataValidator.AddErrorField("transactionTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await IdNumberCore.GetIdNumberById(transactionTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getnextidnum")]
        public async Task<ActionResult> GetNextIdNum(string tranTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(tranTypeId))
                {
                    await DataValidator.AddErrorField("tranTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await IdNumberCore.GetNextIdNum(tranTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getnxtdocnum")]
        public async Task<ActionResult> GetNxtDocNum(string tranTypeId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(tranTypeId))
                {
                    await DataValidator.AddErrorField("tranTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await IdNumberCore.GetNxtDocNum(tranTypeId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createidnumber")]
        public async Task<ActionResult> CreateIdNumber(IdNumberModel idNumber)
        {
            try
            {
                if (idNumber == null)
                {
                    await DataValidator.AddErrorField("idNumber");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await IdNumberCore.CreateIdNumber(idNumber));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateidnumber")]
        public async Task<ActionResult> UpdateIdNumber(IdNumberModel idNumber)
        {
            try
            {
                if (idNumber == null)
                {
                    await DataValidator.AddErrorField("idNumber");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await IdNumberCore.UpdateIdNumber(idNumber));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteidnumber")]
        public async Task<ActionResult> DeleteIdNumber(string transactionTypeId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(transactionTypeId))
                {
                    await DataValidator.AddErrorField("transactionTypeId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await IdNumberCore.DeleteIdNumber(transactionTypeId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("preprintepc")]
        public async Task<ActionResult> PrePrintEPC(int count)
        {
            try
            {
                return Ok(await IdNumberCore.PrePrintEPC(count));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("preprintlpn")]
        public async Task<ActionResult> PrePrintLPN(int count)
        {
            try
            {
                return Ok(await IdNumberCore.PrePrintLPN(count));
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
