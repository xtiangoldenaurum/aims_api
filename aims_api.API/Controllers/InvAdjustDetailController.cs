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
    public class InvAdjustDetailController : ControllerBase
    {
        private IInvAdjustDetailCore InvAdjustDetailCore { get; set; }
        DataValidator DataValidator;
        public InvAdjustDetailController(IInvAdjustDetailCore invAdjustDetailCore)
        {
            InvAdjustDetailCore = invAdjustDetailCore;
            DataValidator = new DataValidator();
        }

        //[HttpGet("getinvadjustdetailbyinvadjustidpaged")]
        //public async Task<ActionResult> GetInvAdjustDetailByInvAdjustIDPaged(string invAdjustId, int pageNum = 1, int pageItem = 100)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(invAdjustId))
        //        {
        //            await DataValidator.AddErrorField("invAdjustId");
        //        }
        //        if (DataValidator.Invalid)
        //        {
        //            return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
        //        }

        //        return Ok(await InvAdjustDetailCore.GetInvAdjustDetailByInvAdjustIDPaged(invAdjustId, pageNum, pageItem));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}

        [HttpGet("getinvadjustdetailbyinvadjustidpagedmod")]
        public async Task<ActionResult> GetInvAdjustDetailByInvAdjustIDPagedMod(string invAdjustId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustId))
                {
                    await DataValidator.AddErrorField("invAdjustId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustDetailCore.GetInvAdjustDetailByInvAdjustIDPagedMod(invAdjustId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustdetailpg")]
        public async Task<ActionResult> GetInvAdjustDetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvAdjustDetailCore.GetInvAdjustDetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustdetailpgsrch")]
        public async Task<ActionResult> GetInvAdjustDetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvAdjustDetailCore.GetInvAdjustDetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvadjustdetailbyid")]
        public async Task<ActionResult> GetInvAdjustDetailById(string invAdjustLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustLineId))
                {
                    await DataValidator.AddErrorField("invAdjustLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustDetailCore.GetInvAdjustDetailById(invAdjustLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvadjustdetail")]
        public async Task<ActionResult> CreateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail)
        {
            try
            {
                if (invAdjustDetail == null)
                {
                    await DataValidator.AddErrorField("invAdjustDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustDetailCore.CreateInvAdjustDetail(invAdjustDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvadjustdetail")]
        public async Task<ActionResult> UpdateInvAdjustDetail(InvAdjustDetailModel invAdjustDetail)
        {
            try
            {
                if (invAdjustDetail == null)
                {
                    await DataValidator.AddErrorField("invAdjustDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustDetailCore.UpdateInvAdjustDetail(invAdjustDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvadjustdetail")]
        public async Task<ActionResult> DeleteInvAdjustDetail(string invAdjustLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustLineId))
                {
                    await DataValidator.AddErrorField("invAdjustLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustDetailCore.DeleteInvAdjustDetail(invAdjustLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvadjustdetailmod")]
        public async Task<ActionResult> DeleteInvAdjustDetailMod(string invAdjustLineId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invAdjustLineId))
                {
                    await DataValidator.AddErrorField("invAdjustLineId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvAdjustDetailCore.DeleteInvAdjustDetailMod(invAdjustLineId, userAccountId));
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
