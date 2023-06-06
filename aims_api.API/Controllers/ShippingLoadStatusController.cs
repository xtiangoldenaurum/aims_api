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

    public class ShippingLoadStatusController : ControllerBase
    {
        private IShippingLoadStatusCore ShippingLoadStatusCore { get; set; }
        DataValidator DataValidator;

        public ShippingLoadStatusController(IShippingLoadStatusCore shippingLoadStatusCore)
        {
            ShippingLoadStatusCore = shippingLoadStatusCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getshippingloadstatuspg")]
        public async Task<ActionResult> GetShippingLoadStatusPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ShippingLoadStatusCore.GetShippingLoadStatusPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getshippingloadstatuspgsrch")]
        public async Task<ActionResult> GetShippingLoadStatusPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ShippingLoadStatusCore.GetShippingLoadStatusPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getshippingloadstatusbyid")]
        public async Task<ActionResult> GetShippingLoadStatusById(string shippingLoadStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(shippingLoadStatusId))
                {
                    await DataValidator.AddErrorField("shippingLoadStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ShippingLoadStatusCore.GetShippingLoadStatusById(shippingLoadStatusId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createshippingloadstatus")]
        public async Task<ActionResult> CreateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus)
        {
            try
            {
                if (shippingLoadStatus == null)
                {
                    await DataValidator.AddErrorField("shippingLoadStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ShippingLoadStatusCore.CreateShippingLoadStatus(shippingLoadStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateshippingloadstatus")]
        public async Task<ActionResult> UpdateShippingLoadStatus(ShippingLoadStatusModel shippingLoadStatus)
        {
            try
            {
                if (shippingLoadStatus == null)
                {
                    await DataValidator.AddErrorField("shippingLoadStatus");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ShippingLoadStatusCore.UpdateShippingLoadStatus(shippingLoadStatus));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteshippingloadstatus")]
        public async Task<ActionResult> DeleteShippingLoadStatus(string shippingLoadStatusId)
        {
            try
            {
                if (string.IsNullOrEmpty(shippingLoadStatusId))
                {
                    await DataValidator.AddErrorField("shippingLoadStatusId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ShippingLoadStatusCore.DeleteShippingLoadStatus(shippingLoadStatusId));
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
