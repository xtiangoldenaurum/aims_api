using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Serilog;

namespace aims_api.API.Controllers
{
    //Web API includes filters to add extra logic before or after action method executes.
    //[Authorize] //This Authorization attribute is everyone can see the detail view but every other action requires the user to identity themselves and sign im
    [Route("api/[controller]")]
    [ApiController]

    public class POController : ControllerBase
    {
        private IPOCore POCore { get; set; }
        DataValidator DataValidator;

        public POController(IPOCore poCore)
        {
            POCore = poCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getpospecial")]
        public async Task<ActionResult> GetPOSpecial(POFilteredMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await POCore.GetPOSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw; //allows to create custom error.
            } 
        }

        [HttpGet("getpoforrcvpaged")]
        public async Task<ActionResult> GetPOForRcvPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await POCore.GetPOForRcvPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpopg")]
        public async Task<ActionResult> GetPOPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await POCore.GetPOPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpopgsrch")]
        public async Task<ActionResult> GetPOPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await POCore.GetPOPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpoheaderbyid")]
        public async Task<ActionResult> GetPOHeaderById(string poId)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.GetPOById(poId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpobyid")]
        public async Task<ActionResult> GetPOById(string poId)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.GetPOByIdMod(poId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createpomod")]
        public async Task<ActionResult> CreatePOMod(POModelMod po)
        {
            try
            {
                if (po == null)
                {
                    await DataValidator.AddErrorField("po");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.CreatePOMod(po));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        //[HttpPost("createpo")]
        //public async Task<ActionResult> CreatePO(POModel po)
        //{
        //    try
        //    {
        //        if (po == null)
        //        {
        //            await DataValidator.AddErrorField("po");
        //        }
        //        if (DataValidator.Invalid)
        //        {
        //            return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
        //        }

        //        return Ok(await POCore.CreatePO(po));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}

        [HttpPost("updatepomod")]
        public async Task<ActionResult> UpdatePOMod(POModelMod po)
        {
            try
            {
                if (po == null)
                {
                    await DataValidator.AddErrorField("po");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.UpdatePOMod(po));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepo")]
        public async Task<ActionResult> DeletePO(string poId)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.DeletePO(poId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("cancelpo")]
        public async Task<ActionResult> CancelPO(string poId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.CancelPO(poId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }


        [HttpPost("forcecancelpo")]
        public async Task<ActionResult> ForceCancelPO(string poId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await POCore.ForceCancelPO(poId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpotemplate")]
        public async Task<ActionResult> GetPOTemplate()
        {
            //csv
            try
            {
                var templatePath = await POCore.GetPOTemplate();
                var fileBytes = await System.IO.File.ReadAllBytesAsync(templatePath);

                return File(fileBytes, "text/csv", "PO_Template.csv");
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
            //    var templatePath = await POCore.DownloadPOTemplate();
            //    var stream = new FileStream(templatePath, FileMode.Open);

            //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PO_Template.csv");
            //}
            //catch (Exception ex)
            //{
            //    Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
            //    return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
            //    throw;
            //}
        }

        [HttpGet("getexportpo")]
        public async Task<ActionResult> GetExportPO()
        {
            try
            {
                return Ok(await POCore.GetExportPO());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }

            //var data = await POCore.ExportPO(); // Fetch data from the repository

            //using (var package = new ExcelPackage())
            //{
            //    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            //    // Set headers
            //    worksheet.Cells[1, 1].Value = "PO No.";
            //    worksheet.Cells[1, 2].Value = "Order Date";
            //    worksheet.Cells[1, 3].Value = "Reference No.";
            //    worksheet.Cells[1, 4].Value = "Supplier";
            //    worksheet.Cells[1, 5].Value = "Order Status";
            //    // Add more headers as needed

            //    // Populate data
            //    var row = 2;
            //    foreach (var item in data)
            //    {
            //        worksheet.Cells[row, 1].Value = item.PoId;
            //        worksheet.Cells[row, 2].Value = item.OrderDate;
            //        worksheet.Cells[row, 3].Value = item.RefNumber;
            //        worksheet.Cells[row, 4].Value = item.SupplierName;
            //        worksheet.Cells[row, 5].Value = item.PoStatusId;
            //        // Add more properties as needed

            //        row++;
            //    }

            //    // Generate the file
            //    var stream = new MemoryStream(package.GetAsByteArray());

            //    // Return the file as an attachment
            //    var response = File(stream, "application/octet-stream");
            //    response.FileDownloadName = "POList.xlsx";
            //    return response;
            //}
        }

        [HttpPost("createbulkpo")]
        public async Task<ActionResult> CreateBulkPO(IFormFile file)
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
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "No file was uploaded."));
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                string? path = "UploadFileFolder/" + file.FileName;

                return Ok(await POCore.CreateBulkPO(file, path));
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
