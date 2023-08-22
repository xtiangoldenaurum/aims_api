using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvCountDetailController : ControllerBase
    {
        private IInvCountDetailCore InvCountDetailCore { get; set; }
        DataValidator DataValidator;
        public InvCountDetailController(IInvCountDetailCore invCountDetailCore)
        {
            InvCountDetailCore = invCountDetailCore;
            DataValidator = new DataValidator();
        }

        #region [HttpGet("getinvcountdetailbyinvcountidpaged")]
        //[HttpGet("getinvcountdetailbyinvcountidpaged")]
        //public async Task<ActionResult> GetInvCountDetailByInvCountIDPaged(string invCountId, int pageNum = 1, int pageItem = 100)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(invCountId))
        //        {
        //            await DataValidator.AddErrorField("invCountId");
        //        }
        //        if (DataValidator.Invalid)
        //        {
        //            return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
        //        }

        //        return Ok(await InvCountDetailCore.GetInvCountDetailByInvCountIDPaged(invCountId, pageNum, pageItem));
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
        //        return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
        //        throw;
        //    }
        //}
        #endregion

        [HttpGet("getinvcountdetailbyinvcountidpagedmod")]
        public async Task<ActionResult> GetInvCountDetailByInvCountIDPagedMod(string invCountId, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvCountDetailCore.GetInvCountDetailByInvCountIDPagedMod(invCountId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountdetailpg")]
        public async Task<ActionResult> GetInvCountDetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvCountDetailCore.GetInvCountDetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountdetailpgsrch")]
        public async Task<ActionResult> GetInvCountDetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvCountDetailCore.GetInvCountDetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getinvcountdetailbyid")]
        public async Task<ActionResult> GetInvCountDetailById(string invCountLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountLineId))
                {
                    await DataValidator.AddErrorField("invCountLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountDetailCore.GetInvCountDetailById(invCountLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createinvcountdetail")]
        public async Task<ActionResult> CreateInvCountDetail(InvCountDetailModel invCountDetail)
        {
            try
            {
                if (invCountDetail == null)
                {
                    await DataValidator.AddErrorField("invCountDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountDetailCore.CreateInvCountDetail(invCountDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateinvcountdetail")]
        public async Task<ActionResult> UpdateInvCountDetail(InvCountDetailModel invCountDetail)
        {
            try
            {
                if (invCountDetail == null)
                {
                    await DataValidator.AddErrorField("invCountDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountDetailCore.UpdateInvCountDetail(invCountDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvcountdetail")]
        public async Task<ActionResult> DeleteInvCountDetail(string invCountLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountLineId))
                {
                    await DataValidator.AddErrorField("invCountLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountDetailCore.DeleteInvCountDetail(invCountLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteinvcountdetailmod")]
        public async Task<ActionResult> DeleteInvCountDetailMod(string invCountLineId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invCountLineId))
                {
                    await DataValidator.AddErrorField("invCountLineId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvCountDetailCore.DeleteInvCountDetailMod(invCountLineId, userAccountId));
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
