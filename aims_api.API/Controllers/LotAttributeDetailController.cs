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

    public class LotAttributeDetailController : ControllerBase
    {
        private ILotAttributeDetailCore LotAttributeDetailCore { get; set; }
        DataValidator DataValidator;

        public LotAttributeDetailController(ILotAttributeDetailCore lotAttributeDetailCore)
        {
            LotAttributeDetailCore = lotAttributeDetailCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getlotattributedetailpg")]
        public async Task<ActionResult> GetLotAttributeDetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await LotAttributeDetailCore.GetLotAttributeDetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlotattributedetailpgsrch")]
        public async Task<ActionResult> GetLotAttributeDetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await LotAttributeDetailCore.GetLotAttributeDetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getlotattributedetailbyid")]
        public async Task<ActionResult> GetLotAttributeDetailById(string lotAttributeId)
        {
            try
            {
                if (string.IsNullOrEmpty(lotAttributeId))
                {
                    await DataValidator.AddErrorField("lotAttributeId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LotAttributeDetailCore.GetLotAttributeDetailById(lotAttributeId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createlotattributedetail")]
        public async Task<ActionResult> CreateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail)
        {
            try
            {
                if (lotAttributeDetail == null)
                {
                    await DataValidator.AddErrorField("lotAttributeDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LotAttributeDetailCore.CreateLotAttributeDetail(lotAttributeDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatelotattributedetail")]
        public async Task<ActionResult> UpdateLotAttributeDetail(LotAttributeDetailModel lotAttributeDetail)
        {
            try
            {
                if (lotAttributeDetail == null)
                {
                    await DataValidator.AddErrorField("lotAttributeDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LotAttributeDetailCore.UpdateLotAttributeDetail(lotAttributeDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletelotattributedetail")]
        public async Task<ActionResult> DeleteLotAttributeDetail(string lotAttributeId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(lotAttributeId))
                {
                    await DataValidator.AddErrorField("lotAttributeId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await LotAttributeDetailCore.DeleteLotAttributeDetail(lotAttributeId, userAccountId));
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
