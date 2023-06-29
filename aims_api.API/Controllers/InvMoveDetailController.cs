using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace aims_api.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvMoveDetailController : ControllerBase
    {
        private IInvMoveDetailCore InvMoveDetailCore { get; set; }
        DataValidator DataValidator;
        public InvMoveDetailController(IInvMoveDetailCore invMoveDetailCore)
        {
            InvMoveDetailCore = invMoveDetailCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getinvmovedetailbyinvmoveidpaged")]
        public async Task<ActionResult> GetInvMoveDetailByInvMoveIDPaged(string invMoveId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.GetInvMoveDetailByInvMoveIDPaged(invMoveId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailbypoidpagedmod")]
        public async Task<ActionResult> GetInvMoveDetailByPoIDPagedMod(string invMoveId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveId))
                {
                    await DataValidator.AddErrorField("invMoveId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.GetInvMoveDetailByInvMoveIDPagedMod(invMoveId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailpg")]
        public async Task<ActionResult> GetInvMoveDetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await InvMoveDetailCore.GetInvMoveDetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailpgsrch")]
        public async Task<ActionResult> GetInvMoveDetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await InvMoveDetailCore.GetInvMoveDetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailbyid")]
        public async Task<ActionResult> GetInvMoveDetailById(string invMoveLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveLineId))
                {
                    await DataValidator.AddErrorField("invMoveLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.GetInvMoveDetailById(invMoveLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createpodetail")]
        public async Task<ActionResult> CreateInvMoveDetail(InvMoveDetailModel invMoveDetail)
        {
            try
            {
                if (invMoveDetail == null)
                {
                    await DataValidator.AddErrorField("invMoveDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.CreateInvMoveDetail(invMoveDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatepodetail")]
        public async Task<ActionResult> UpdateInvMoveDetail(InvMoveDetailModel invMoveDetail)
        {
            try
            {
                if (invMoveDetail == null)
                {
                    await DataValidator.AddErrorField("invMoveDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.UpdateInvMoveDetail(invMoveDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepodetail")]
        public async Task<ActionResult> DeleteInvMoveDetail(string invMoveLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveLineId))
                {
                    await DataValidator.AddErrorField("invMoveLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.DeleteInvMoveDetail(invMoveLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepodetailmod")]
        public async Task<ActionResult> DeleteInvMoveDetailMod(string invMoveLineId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(invMoveLineId))
                {
                    await DataValidator.AddErrorField("invMoveLineId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await InvMoveDetailCore.DeleteInvMoveDetailMod(invMoveLineId, userAccountId));
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
