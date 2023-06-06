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

    public class SOController : ControllerBase
    {
        private ISOCore SOCore { get; set; }
        DataValidator DataValidator;

        public SOController(ISOCore soCore)
        {
            SOCore = soCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getsopg")]
        public async Task<ActionResult> GetSOPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await SOCore.GetSOPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getsopgsrch")]
        public async Task<ActionResult> GetSOPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await SOCore.GetSOPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getsobyid")]
        public async Task<ActionResult> GetSOById(string soId)
        {
            try
            {
                if (string.IsNullOrEmpty(soId))
                {
                    await DataValidator.AddErrorField("soId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOCore.GetSOById(soId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createso")]
        public async Task<ActionResult> CreateSO(SOModel so)
        {
            try
            {
                if (so == null)
                {
                    await DataValidator.AddErrorField("so");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOCore.CreateSO(so));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateso")]
        public async Task<ActionResult> UpdateSO(SOModel so)
        {
            try
            {
                if (so == null)
                {
                    await DataValidator.AddErrorField("so");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOCore.UpdateSO(so));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteso")]
        public async Task<ActionResult> DeleteSO(string soId)
        {
            try
            {
                if (string.IsNullOrEmpty(soId))
                {
                    await DataValidator.AddErrorField("soId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await SOCore.DeleteSO(soId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("downloadsotemplate")]
        public async Task<ActionResult> DownloadSOTemplate()
        {
            try
            {
                var templatePath = await SOCore.DownloadSOTemplate();
                var stream = new FileStream(templatePath, FileMode.Open);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SO_Template.xlsx");
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("exportso")]
        public async Task<ActionResult> ExportSO()
        {
            try
            {
                return Ok(await SOCore.ExportSO());
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
