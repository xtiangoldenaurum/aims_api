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

    public class InventoryController : ControllerBase
    {
        private IInventoryCore InventoryCore { get; set; }
        DataValidator DataValidator;

        public InventoryController(IInventoryCore inventoryCore)
        {
            InventoryCore = inventoryCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinventorypg")]
        public async Task<ActionResult> GetInventoryPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InventoryCore.GetInventoryPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinventorypgsrch")]
        public async Task<ActionResult> GetInventoryPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InventoryCore.GetInventoryPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinventorybyid")]
        public async Task<ActionResult> GetInventoryById(string inventoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(inventoryId))
                {
                    await DataValidator.AddErrorField("inventoryId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryCore.GetInventoryById(inventoryId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinventory")]
        public async Task<ActionResult> CreateInventory(InventoryModel inventory)
        {
            try
            {
                if (inventory == null)
                {
                    await DataValidator.AddErrorField("inventory");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryCore.CreateInventory(inventory));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinventory")]
        public async Task<ActionResult> UpdateInventory(InventoryModel inventory)
        {
            try
            {
                if (inventory == null)
                {
                    await DataValidator.AddErrorField("inventory");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryCore.UpdateInventory(inventory));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinventory")]
        public async Task<ActionResult> DeleteInventory(string inventoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(inventoryId))
                {
                    await DataValidator.AddErrorField("inventoryId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryCore.DeleteInventory(inventoryId));
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
