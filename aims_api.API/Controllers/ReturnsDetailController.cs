using aims_api.Cores.Implementation;
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

    public class ReturnsDetailController : ControllerBase
    {
        private IReturnsDetailCore ReturnsDetailCore { get; set; }
        DataValidator DataValidator;

        public ReturnsDetailController(IReturnsDetailCore returnsDetailCore)
        {
            ReturnsDetailCore = returnsDetailCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getretdetailbyretidpagedmod")]
        public async Task<ActionResult> GetRetDetailByRetIdPagedMod(string returnsId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(returnsId))
                {
                    await DataValidator.AddErrorField("returnsId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsDetailCore.GetRetDetailByRetIdPagedMod(returnsId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsdetailpg")]
        public async Task<ActionResult> GetReturnsDetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ReturnsDetailCore.GetReturnsDetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsdetailpgsrch")]
        public async Task<ActionResult> GetReturnsDetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ReturnsDetailCore.GetReturnsDetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getreturnsdetailbyid")]
        public async Task<ActionResult> GetReturnsDetailById(string returnsLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(returnsLineId))
                {
                    await DataValidator.AddErrorField("returnsLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsDetailCore.GetReturnsDetailById(returnsLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createreturnsdetail")]
        public async Task<ActionResult> CreateReturnsDetail(ReturnsDetailModel returnsDetail)
        {
            try
            {
                if (returnsDetail == null)
                {
                    await DataValidator.AddErrorField("returnsDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsDetailCore.CreateReturnsDetail(returnsDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatereturnsdetail")]
        public async Task<ActionResult> UpdateReturnsDetail(ReturnsDetailModel returnsDetail)
        {
            try
            {
                if (returnsDetail == null)
                {
                    await DataValidator.AddErrorField("returnsDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsDetailCore.UpdateReturnsDetail(returnsDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletereturnsdetail")]
        public async Task<ActionResult> DeleteReturnsDetail(string returnsLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(returnsLineId))
                {
                    await DataValidator.AddErrorField("returnsLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsDetailCore.DeleteReturnsDetail(returnsLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }
        
        [HttpDelete("deletereturnsdetailmod")]
        public async Task<ActionResult> DeleteReturnsDetailMod(string returnsLineId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(returnsLineId))
                {
                    await DataValidator.AddErrorField("returnsLineId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ReturnsDetailCore.DeleteReturnsDetailMod(returnsLineId, userAccountId));
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
