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

    public class InventoryStatusController : ControllerBase
    {
        private IInventoryStatusCore InventoryStatusCore { get; set; }
        DataValidator DataValidator;

        public InventoryStatusController(IInventoryStatusCore inventoryStatusCore)
        {
            InventoryStatusCore = inventoryStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinventorystatuspg")]
        public async Task<ActionResult> GetInventoryStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InventoryStatusCore.GetInventoryStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinventorystatuspgsrch")]
        public async Task<ActionResult> GetInventoryStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InventoryStatusCore.GetInventoryStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinventorystatusbyid")]
        public async Task<ActionResult> GetInventoryStatusById(string inventoryStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(inventoryStatusId))
                {
                    await DataValidator.AddErrorField("inventoryStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryStatusCore.GetInventoryStatusById(inventoryStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinventorystatus")]
        public async Task<ActionResult> CreatetblName(InventoryStatusModel inventoryStatus)
        {
            try
            {
                if (inventoryStatus == null)
                {
                    await DataValidator.AddErrorField("inventoryStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryStatusCore.CreateInventoryStatus(inventoryStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinventorystatus")]
        public async Task<ActionResult> UpdateInventoryStatus(InventoryStatusModel inventoryStatus)
        {
            try
            {
                if (inventoryStatus == null)
                {
                    await DataValidator.AddErrorField("inventoryStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryStatusCore.UpdateInventoryStatus(inventoryStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinventorystatus")]
        public async Task<ActionResult> DeleteInventoryStatus(string inventoryStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(inventoryStatusId))
                {
                    await DataValidator.AddErrorField("inventoryStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryStatusCore.DeleteInventoryStatus(inventoryStatusId));
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
