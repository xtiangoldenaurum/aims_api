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

    public class PODetailController : ControllerBase
    {
        private IPODetailCore PODetailCore { get; set; }
        DataValidator DataValidator;

        public PODetailController(IPODetailCore poDetailCore)
        {
            PODetailCore = poDetailCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getpodetailbypoidpaged")]
        public async Task<ActionResult> GetPODetailByPoIDPaged(string poId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.GetPODetailByPoIDPaged(poId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailbypoidpagedmod")]
        public async Task<ActionResult> GetPODetailByPoIDPagedMod(string poId, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(poId))
                {
                    await DataValidator.AddErrorField("poId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.GetPODetailByPoIDPagedMod(poId, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailpg")]
        public async Task<ActionResult> GetPODetailPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await PODetailCore.GetPODetailPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailpgsrch")]
        public async Task<ActionResult> GetPODetailPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await PODetailCore.GetPODetailPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getpodetailbyid")]
        public async Task<ActionResult> GetPODetailById(string poLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(poLineId))
                {
                    await DataValidator.AddErrorField("poLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.GetPODetailById(poLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createpodetail")]
        public async Task<ActionResult> CreatePODetail(PODetailModel poDetail)
        {
            try
            {
                if (poDetail == null)
                {
                    await DataValidator.AddErrorField("poDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.CreatePODetail(poDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updatepodetail")]
        public async Task<ActionResult> UpdatePODetail(PODetailModel poDetail)
        {
            try
            {
                if (poDetail == null)
                {
                    await DataValidator.AddErrorField("poDetail");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.UpdatePODetail(poDetail));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepodetail")]
        public async Task<ActionResult> DeletePODetail(string poLineId)
        {
            try
            {
                if (string.IsNullOrEmpty(poLineId))
                {
                    await DataValidator.AddErrorField("poLineId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.DeletePODetail(poLineId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deletepodetailmod")]
        public async Task<ActionResult> DeletePODetailMod(string poLineId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(poLineId))
                {
                    await DataValidator.AddErrorField("poLineId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await PODetailCore.DeletePODetailMod(poLineId, userAccountId));
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
