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

    public class UniqueTagsController : ControllerBase
    {
        private IUniqueTagsCore UniqueTagsCore { get; set; }
        DataValidator DataValidator;

        public UniqueTagsController(IUniqueTagsCore uniqueTagsCore)
        {
            UniqueTagsCore = uniqueTagsCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getuniquetagspg")]
        public async Task<ActionResult> GetUniqueTagsPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await UniqueTagsCore.GetUniqueTagsPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuniquetagspgsrch")]
        public async Task<ActionResult> GetUniqueTagsPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await UniqueTagsCore.GetUniqueTagsPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getuniquetagsbyid")]
        public async Task<ActionResult> GetUniqueTagsById(int uniqueTagId)
        {
            try
            {
                if (uniqueTagId < 1)
                {
                    await DataValidator.AddErrorField("uniqueTagId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UniqueTagsCore.GetUniqueTagsById(uniqueTagId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createuniquetags")]
        public async Task<ActionResult> CreateUniqueTags(UniqueTagsModel uniqueTags)
        {
            try
            {
                if (uniqueTags == null)
                {
                    await DataValidator.AddErrorField("uniqueTags");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UniqueTagsCore.CreateUniqueTags(uniqueTags));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateuniquetags")]
        public async Task<ActionResult> UpdateUniqueTags(UniqueTagsModel uniqueTags)
        {
            try
            {
                if (uniqueTags == null)
                {
                    await DataValidator.AddErrorField("uniqueTags");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UniqueTagsCore.UpdateUniqueTags(uniqueTags));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteuniquetags")]
        public async Task<ActionResult> DeleteUniqueTags(int uniqueTagId)
        {
            try
            {
                if (uniqueTagId < 1)
                {
                    await DataValidator.AddErrorField("uniqueTagId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await UniqueTagsCore.DeleteUniqueTags(uniqueTagId));
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
