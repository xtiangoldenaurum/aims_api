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

    public class WhTransferController : ControllerBase
    {
        private IWhTransferCore WhTransferCore { get; set; }
        DataValidator DataValidator;

        public WhTransferController(IWhTransferCore whTransferCore)
        {
            WhTransferCore = whTransferCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getwhtransferspecial")]
        public async Task<ActionResult> GetWhTransferSpecial(WhTransferFilteredMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await WhTransferCore.GetWhTransferSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferforrcvpaged")]
        public async Task<ActionResult> GetWhTransferForRcvPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await WhTransferCore.GetWhTransferForRcvPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferpgsrch")]
        public async Task<ActionResult> GetWhTransferPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await WhTransferCore.GetWhTransferPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferheaderbyid")]
        public async Task<ActionResult> GetWhTransferHeaderById(string whTransferId)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransferId))
                {
                    await DataValidator.AddErrorField("whTransferId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferCore.GetWhTransferById(whTransferId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferbyid")]
        public async Task<ActionResult> GetWhTransferById(string whTransferId)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransferId))
                {
                    await DataValidator.AddErrorField("whTransferId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferCore.GetWhTransferByIdMod(whTransferId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createwhtransfermod")]
        public async Task<ActionResult> CreateWhTransferMod(WhTransferModelMod whTransfer)
        {
            try
            {
                if (whTransfer == null)
                {
                    await DataValidator.AddErrorField("whTransfer");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferCore.CreateWhTransferMod(whTransfer));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatewhtransfermod")]
        public async Task<ActionResult> UpdateWhTransferMod(WhTransferModelMod whTransfer)
        {
            try
            {
                if (whTransfer == null)
                {
                    await DataValidator.AddErrorField("whTransfer");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferCore.UpdateWhTransferMod(whTransfer));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletewhtransfer")]
        public async Task<ActionResult> DeleteWhTransfer(string whTransferId)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransferId))
                {
                    await DataValidator.AddErrorField("whTransferId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferCore.DeleteWhTransfer(whTransferId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getdistinctwhtransfrom")]
        public async Task<ActionResult> GetDistinctWhTransFrom()
        {
            try
            {
                return Ok(await WhTransferCore.GetDistinctWhTransFrom());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("downloadwhtransfertemplate")]
        public async Task<ActionResult> DownloadWhTransferTemplate()
        {
            //csv
            try
            {
                var templatePath = await WhTransferCore.DownloadWhTransferTemplate();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(templatePath);

                return File(fileBytes, "text/csv", "WhTransfer_Template.csv");
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
            //xlsx
            //try
            //{
            //    var templatePath = await WhTransferCore.DownloadWhTransferTemplate();
            //    var stream = new FileStream(templatePath, FileMode.Open);

            //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WhTransfer_Template.xlsx");
            //}
            //catch (Exception ex)
            //{
            //    Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
            //    return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
            //    throw;
            //}
        }

        [HttpGet("exportwhtransfer")]
        public async Task<ActionResult> ExportWhTransfer()
        {
            try
            {
                return Ok(await WhTransferCore.ExportWhTransfer());
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
