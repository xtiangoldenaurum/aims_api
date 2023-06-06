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

    public class ProductConditionController : ControllerBase
    {
        private IProductConditionCore ProductConditionCore { get; set; }
        DataValidator DataValidator;

        public ProductConditionController(IProductConditionCore productConditionCore)
        {
            ProductConditionCore = productConditionCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getproductconditionpg")]
        public async Task<ActionResult> GetProductConditionPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductConditionCore.GetProductConditionPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductconditionpgsrch")]
        public async Task<ActionResult> GetProductConditionPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ProductConditionCore.GetProductConditionPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductconditionbyid")]
        public async Task<ActionResult> GetProductConditionById(string productConditionId)
        {
            try
            {
                if (string.IsNullOrEmpty(productConditionId))
                {
                    await DataValidator.AddErrorField("productConditionId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductConditionCore.GetProductConditionById(productConditionId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createproductcondition")]
        public async Task<ActionResult> CreateProductCondition(ProductConditionModel productCondition)
        {
            try
            {
                if (productCondition == null)
                {
                    await DataValidator.AddErrorField("productCondition");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductConditionCore.CreateProductCondition(productCondition));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateproductcondition")]
        public async Task<ActionResult> UpdateProductCondition(ProductConditionModel productCondition)
        {
            try
            {
                if (productCondition == null)
                {
                    await DataValidator.AddErrorField("productCondition");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductConditionCore.UpdateProductCondition(productCondition));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteproductcondition")]
        public async Task<ActionResult> DeleteProductCondition(string productConditionId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(productConditionId))
                {
                    await DataValidator.AddErrorField("productConditionId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductConditionCore.DeleteProductCondition(productConditionId, userAccountId));
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
