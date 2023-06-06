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

    public class WhTransferDetailController : ControllerBase
    {
        private IWhTransferDetailCore WhTransferDetailCore { get; set; }
        DataValidator DataValidator;

        public WhTransferDetailController(IWhTransferDetailCore whTransferDetailCore)
        {
            WhTransferDetailCore = whTransferDetailCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("GetWhTransDetailByTrnasIdPagedMod")]
        public async Task<ActionResult> GetWhTransDetailByTrnasIdPagedMod(string whTransId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransId))
                {
                    await DataValidator.AddErrorField("whTransId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferDetailCore.GetWhTransDetailByTrnasIdPagedMod(whTransId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferdetailpg")]
        public async Task<ActionResult> GetWhTransferDetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await WhTransferDetailCore.GetWhTransferDetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferdetailpgsrch")]
        public async Task<ActionResult> GetWhTransferDetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await WhTransferDetailCore.GetWhTransferDetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getwhtransferdetailbyid")]
        public async Task<ActionResult> GetWhTransferDetailById(string whTransferLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransferLineId))
                {
                    await DataValidator.AddErrorField("whTransferLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferDetailCore.GetWhTransferDetailById(whTransferLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createwhtransferdetail")]
        public async Task<ActionResult> CreateWhTransferDetail(WhTransferDetailModel whTransferDetail)
        {
            try
            {
                if (whTransferDetail == null)
                {
                    await DataValidator.AddErrorField("whTransferDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferDetailCore.CreateWhTransferDetail(whTransferDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatewhtransferdetail")]
        public async Task<ActionResult> UpdateWhTransferDetail(WhTransferDetailModel whTransferDetail)
        {
            try
            {
                if (whTransferDetail == null)
                {
                    await DataValidator.AddErrorField("whTransferDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferDetailCore.UpdateWhTransferDetail(whTransferDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletewhtransferdetail")]
        public async Task<ActionResult> DeleteWhTransferDetail(string whTransferLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransferLineId))
                {
                    await DataValidator.AddErrorField("whTransferLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferDetailCore.DeleteWhTransferDetail(whTransferLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletewhtransferdetailmod")]
        public async Task<ActionResult> DeleteWhTransferDetailMod(string whTransferLineId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(whTransferLineId))
                {
                    await DataValidator.AddErrorField("whTransferLineId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await WhTransferDetailCore.DeleteWhTransferDetailMmod(whTransferLineId, userAccountId));
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
