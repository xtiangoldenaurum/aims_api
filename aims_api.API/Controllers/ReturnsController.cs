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

    public class ReturnsController : ControllerBase
    {
        private IReturnsCore ReturnsCore { get; set; }
        DataValidator DataValidator;

        public ReturnsController(IReturnsCore returnsCore)
        {
            ReturnsCore = returnsCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getreturnsspecial")]
        public async Task<ActionResult> GetReturnsSpecial(ReturnsFilteredMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ReturnsCore.GetReturnsSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsforrcvpaged")]
        public async Task<ActionResult> GetReturnsForRcvPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ReturnsCore.GetReturnsForRcvPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnspg")]
        public async Task<ActionResult> GetReturnsPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ReturnsCore.GetReturnsPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnspgsrch")]
        public async Task<ActionResult> GetReturnsPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ReturnsCore.GetReturnsPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsheaderbyid")]
        public async Task<ActionResult> GetReturnsHeaderById(string returnsId)
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

                return Ok(await ReturnsCore.GetReturnsById(returnsId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsbyid")]
        public async Task<ActionResult> GetReturnsById(string returnsId)
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

                return Ok(await ReturnsCore.GetReturnsByIdMod(returnsId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createreturnsmod")]
        public async Task<ActionResult> CreateReturnsMod(ReturnsModelMod returns)
        {
            try
            {
                if (returns == null)
                {
                    await DataValidator.AddErrorField("returns");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsCore.CreateReturnsMod(returns));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatereturnsmod")]
        public async Task<ActionResult> UpdateReturnsMod(ReturnsModelMod returns)
        {
            try
            {
                if (returns == null)
                {
                    await DataValidator.AddErrorField("returns");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsCore.UpdateReturnsMod(returns));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        //[HttpPost("createreturns")]
        //public async Task<ActionResult> CreateReturns(ReturnsModel returns)
        //{
        //    try
        //    {
        //        if (returns == null)
        //        {
        //            await DataValidator.AddErrorField("returns");
        //        }
        //        if (DataValidator.Invalid)
        //        {
        //            return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
        //        }

        //        return Ok(await ReturnsCore.CreateReturns(returns));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}

        //[HttpPost("updatereturns")]
        //public async Task<ActionResult> UpdateReturns(ReturnsModel returns)
        //{
        //    try
        //    {
        //        if (returns == null)
        //        {
        //            await DataValidator.AddErrorField("returns");
        //        }
        //        if (DataValidator.Invalid)
        //        {
        //            return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
        //        }

        //        return Ok(await ReturnsCore.UpdateReturns(returns));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}

        [HttpDelete("deletereturns")]
        public async Task<ActionResult> DeleteReturns(string returnsId)
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

                return Ok(await ReturnsCore.DeleteReturns(returnsId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getdistinctstorefrom")]
        public async Task<ActionResult> GetDistinctStoreFrom()
        {
            try
            {
                return Ok(await ReturnsCore.GetDistinctStoreFrom());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnstransfer")]
        public async Task<ActionResult> GetReturnsTransferTemplate()
        {
            //csv
            try
            {
                var templatePath = await ReturnsCore.GetReturnsTransferTemplate();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(templatePath);

                return File(fileBytes, "text/csv", "ReturnsTransfer_Template.csv");
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getexportreturnstransfer")]
        public async Task<ActionResult> GetExportReturnsTransfer()
        {
            try
            {
                return Ok(await ReturnsCore.GetExportReturnsTransfer());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createbulkso")]
        public async Task<ActionResult> CreateBulkReturns(IFormFile file)
        {
            try
            {
                if (Path.GetExtension(file.FileName) != ".csv" && Path.GetExtension(file.FileName) != ".xlsx")
                {
                    return BadRequest("Invalid File Type. Only CSV or XLSX files are allowed."); //Error: Bad Request(Request Body Message)
                }
                if (file == null || file.Length <= 0)
                {
                    await DataValidator.AddErrorField("file");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                string path = "UploadFileFolder/" + file.FileName;

                return Ok(await ReturnsCore.CreateBulkReturns(file, path));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
            finally
            {
                // Delete uploaded files
                string[] uploadFiles = Directory.GetFiles("UploadFileFolder/");
                foreach (string uploadFile in uploadFiles)
                {
                    System.IO.File.Delete(uploadFile);
                    Console.WriteLine($"{uploadFile} is deleted.");
                }
            }
        }
    }
}
