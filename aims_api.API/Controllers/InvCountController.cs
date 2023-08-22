using aims_api.Cores.Implementation;
using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvCountController : ControllerBase
    {
        private IInvCountCore InvCountCore { get; set; }
        DataValidator DataValidator;
        public InvCountController(IInvCountCore invCountCore)
        {
            InvCountCore = invCountCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getinvcountspecial")]
        public async Task<ActionResult> GetInvCountSpecial(InvCountFilteredMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvCountCore.GetInvCountSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw; //allows to create custom error.
            }
        }

        [HttpGet("getinvcountforcntpaged")]
        public async Task<ActionResult> GetInvCountForMvPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvCountCore.GetInvCountForCntPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountpg")]
        public async Task<ActionResult> GetInvCountPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvCountCore.GetInvCountPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountpgsrch")]
        public async Task<ActionResult> GetInvCountPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvCountCore.GetInvCountPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountheaderbyid")]
        public async Task<ActionResult> GetInvCountHeaderById(string invCountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountId))
                {
                    await DataValidator.AddErrorField("invCountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountCore.GetInvCountById(invCountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountbyid")]
        public async Task<ActionResult> GetInvCountById(string invCountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountId))
                {
                    await DataValidator.AddErrorField("invCountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountCore.GetInvCountByIdMod(invCountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvcountmod")]
        public async Task<ActionResult> CreateInvCountMod(InvCountModelMod invCount)
        {
            try
            {
                if (invCount == null)
                {
                    await DataValidator.AddErrorField("invCount");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountCore.CreateInvCountMod(invCount));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvcountmod")]
        public async Task<ActionResult> UpdateInvCountMod(InvCountModelMod invCount)
        {
            try
            {
                if (invCount == null)
                {
                    await DataValidator.AddErrorField("invCount");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountCore.UpdateInvCountMod(invCount));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvcount")]
        public async Task<ActionResult> DeleteInvCount(string invCountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountId))
                {
                    await DataValidator.AddErrorField("invCountId");
                }

                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountCore.DeleteInvCount(invCountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        #region cancelinvmove
        [HttpPost("cancelinvcount")]
        public async Task<ActionResult> CancelInvCount(string invCountId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountId))
                {
                    await DataValidator.AddErrorField("invCountId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountCore.CancelInvCount(invCountId, userAccountId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }
        #endregion

        #region forcecancelinvmove
        //[HttpPost("forcecancelinvcount")]
        //public async Task<ActionResult> ForceCancelInvCount(string invCountId, string userAccountId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(invCountId))
        //        {
        //            await DataValidator.AddErrorField("invCountId");
        //        }
        //        if (string.IsNullOrEmpty(userAccountId))
        //        {
        //            await DataValidator.AddErrorField("userAccountId");
        //        }
        //        if (DataValidator.Invalid)
        //        {
        //            return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
        //        }

        //        return Ok(await InvCountCore.ForceCancelInvCount(invCountId, userAccountId));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}
        #endregion

    }
}
