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

    public class InventoryHistoryController : ControllerBase
    {
        private IInventoryHistoryCore InventoryHistoryCore { get; set; }
        DataValidator DataValidator;

        public InventoryHistoryController(IInventoryHistoryCore inventoryHistoryCore)
        {
            InventoryHistoryCore = inventoryHistoryCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinventoryhistorypg")]
        public async Task<ActionResult> GetInventoryHistoryPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InventoryHistoryCore.GetInventoryHistoryPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinventoryhistorypgsrch")]
        public async Task<ActionResult> GetInventoryHistoryPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InventoryHistoryCore.GetInventoryHistoryPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinventoryhistorybyid")]
        public async Task<ActionResult> GetInventoryHistoryById(string inventoryId)
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

                return Ok(await InventoryHistoryCore.GetInventoryHistoryById(inventoryId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinventoryhistory")]
        public async Task<ActionResult> CreateInventoryHistory(InventoryHistoryModel inventoryHistory)
        {
            try
            {
                if (inventoryHistory == null)
                {
                    await DataValidator.AddErrorField("inventoryHistory");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryHistoryCore.CreateInventoryHistory(inventoryHistory));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinventoryhistory")]
        public async Task<ActionResult> UpdateInventoryHistory(InventoryHistoryModel inventoryHistory)
        {
            try
            {
                if (inventoryHistory == null)
                {
                    await DataValidator.AddErrorField("inventoryHistory");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InventoryHistoryCore.UpdateInventoryHistory(inventoryHistory));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinventoryhistory")]
        public async Task<ActionResult> DeleteInventoryHistory(string inventoryId)
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

                return Ok(await InventoryHistoryCore.DeleteInventoryHistory(inventoryId));
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
