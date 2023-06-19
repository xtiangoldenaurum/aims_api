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

        [HttpPost("createsomod")]
        public async Task<ActionResult> CreateSO(SOModelMod so)
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

                return Ok(await SOCore.CreateSOMod(so));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatesomod")]
        public async Task<ActionResult> UpdateSOMod(SOModelMod so)
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

                return Ok(await SOCore.UpdateSOMod(so));
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

        [HttpGet("getsotemplate")]
        public async Task<ActionResult> GetSOTemplate()
        {
            //csv
            try
            {
                var templatePath = await SOCore.GetSOTemplate();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(templatePath);

                return File(fileBytes, "text/csv", "SO_Template.csv");
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getexportso")]
        public async Task<ActionResult> GetExportSO()
        {
            try
            {
                return Ok(await SOCore.GetExportSO());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createbulkso")]
        public async Task<ActionResult> CreateBulkSO(IFormFile file)
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

                return Ok(await SOCore.CreateBulkSO(file, path));
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
