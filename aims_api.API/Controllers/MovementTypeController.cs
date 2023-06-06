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

    public class MovementTypeController : ControllerBase
    {
        private IMovementTypeCore MovementTypeCore { get; set; }
        DataValidator DataValidator;

        public MovementTypeController(IMovementTypeCore movementTypeCore)
        {
            MovementTypeCore = movementTypeCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getmovementtypepg")]
        public async Task<ActionResult> GetMovementTypePg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await MovementTypeCore.GetMovementTypePg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmovementtypepgsrch")]
        public async Task<ActionResult> GetMovementTypePgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await MovementTypeCore.GetMovementTypePgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getmovementtypebyid")]
        public async Task<ActionResult> GetMovementTypeById(string movementTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTypeId))
                {
                    await DataValidator.AddErrorField("movementTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTypeCore.GetMovementTypeById(movementTypeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createmovementtype")]
        public async Task<ActionResult> CreateMovementType(MovementTypeModel movementType)
        {
            try
            {
                if (movementType == null)
                {
                    await DataValidator.AddErrorField("movementType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTypeCore.CreateMovementType(movementType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatemovementtype")]
        public async Task<ActionResult> UpdateMovementType(MovementTypeModel movementType)
        {
            try
            {
                if (movementType == null)
                {
                    await DataValidator.AddErrorField("movementType");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTypeCore.UpdateMovementType(movementType));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletemovementtype")]
        public async Task<ActionResult> DeleteMovementType(string movementTypeId)
        {
            try
            {
                if (string.IsNullOrEmpty(movementTypeId))
                {
                    await DataValidator.AddErrorField("movementTypeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await MovementTypeCore.DeleteMovementType(movementTypeId));
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
