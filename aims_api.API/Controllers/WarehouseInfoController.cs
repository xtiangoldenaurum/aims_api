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

    public class WarehouseInfoController : ControllerBase
    {
        private IWarehouseInfoCore WarehouseInfoCore { get; set; }
        DataValidator DataValidator;

        public WarehouseInfoController(IWarehouseInfoCore warehouseInfoCore)
        {
            WarehouseInfoCore = warehouseInfoCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getwarehouseinfopg")]
        public async Task<ActionResult> GetWarehouseInfoPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await WarehouseInfoCore.GetWarehouseInfoPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwarehouseinfopgsrch")]
        public async Task<ActionResult> GetWarehouseInfoPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await WarehouseInfoCore.GetWarehouseInfoPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwarehouseinfobyid")]
        public async Task<ActionResult> GetWarehouseInfoById(string warehouseId)
        {
            try
            {
                if (string.IsNullOrEmpty(warehouseId))
                {
                    await DataValidator.AddErrorField("warehouseId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WarehouseInfoCore.GetWarehouseInfoById(warehouseId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createwarehouseinfo")]
        public async Task<ActionResult> CreateWarehouseInfo(WarehouseInfoModel warehouseInfo)
        {
            try
            {
                if (warehouseInfo == null)
                {
                    await DataValidator.AddErrorField("warehouseInfo");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WarehouseInfoCore.CreateWarehouseInfo(warehouseInfo));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatewarehouseinfo")]
        public async Task<ActionResult> UpdateWarehouseInfo(WarehouseInfoModel warehouseInfo)
        {
            try
            {
                if (warehouseInfo == null)
                {
                    await DataValidator.AddErrorField("warehouseInfo");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WarehouseInfoCore.UpdateWarehouseInfo(warehouseInfo));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletewarehouseinfo")]
        public async Task<ActionResult> DeleteWarehouseInfo(string warehouseId)
        {
            try
            {
                if (string.IsNullOrEmpty(warehouseId))
                {
                    await DataValidator.AddErrorField("warehouseId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WarehouseInfoCore.DeleteWarehouseInfo(warehouseId));
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
