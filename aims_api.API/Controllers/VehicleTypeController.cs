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

    public class VehicleTypeController : ControllerBase
    {
        private IVehicleTypeCore VehicleTypeCore { get; set; }
        DataValidator DataValidator;

        public VehicleTypeController(IVehicleTypeCore vehicleTypeCore)
        {
            VehicleTypeCore = vehicleTypeCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getvehicletypepg")]
        public async Task<ActionResult> GetVehicleTypePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await VehicleTypeCore.GetVehicleTypePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getvehicletypepgsrch")]
        public async Task<ActionResult> GetVehicleTypePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await VehicleTypeCore.GetVehicleTypePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getvehicletypebyid")]
        public async Task<ActionResult> GetVehicleTypeById(string vehicleTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleTypeId))
                {
                    await DataValidator.AddErrorField("vehicleTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await VehicleTypeCore.GetVehicleTypeById(vehicleTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createvehicletype")]
        public async Task<ActionResult> CreateVehicleType(VehicleTypeModel vehicleType)
        {
            try
            {
                if (vehicleType == null)
                {
                    await DataValidator.AddErrorField("vehicleType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await VehicleTypeCore.CreateVehicleType(vehicleType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatevehicletype")]
        public async Task<ActionResult> UpdateVehicleType(VehicleTypeModel vehicleType)
        {
            try
            {
                if (vehicleType == null)
                {
                    await DataValidator.AddErrorField("vehicleType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await VehicleTypeCore.UpdateVehicleType(vehicleType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletevehicletype")]
        public async Task<ActionResult> DeleteVehicleType(string vehicleTypeId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(vehicleTypeId))
                {
                    await DataValidator.AddErrorField("vehicleTypeId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await VehicleTypeCore.DeleteVehicleType(vehicleTypeId, userAccountId));
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
