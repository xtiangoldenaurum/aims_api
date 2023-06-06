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

    public class ProductUserFieldController : ControllerBase
    {
        private IProductUserFieldCore ProdUfieldCore { get; set; }
        DataValidator DataValidator;

        public ProductUserFieldController(IProductUserFieldCore prodUFieldCore)
        {
            ProdUfieldCore = prodUFieldCore;
            DataValidator = new DataValidator();
        }

        [HttpGet("getproductufieldbyid")]
        public async Task<ActionResult> GetProductUFieldById(string sku)
        {
            try
            {
                if (string.IsNullOrEmpty(sku))
                {
                    await DataValidator.AddErrorField("sku");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }
                return Ok(await ProdUfieldCore.GetProductUFieldById(sku));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductufields")]
        public async Task<ActionResult> GetProductUFields()
        {
            try
            {
                return Ok(await ProdUfieldCore.GetProductUFields());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createproductufield")]
        public async Task<ActionResult> CreateProductUField(string fieldName, string createdBy)
        {
            try
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    await DataValidator.AddErrorField("fieldName");
                }
                if (string.IsNullOrEmpty(createdBy))
                {
                    await DataValidator.AddErrorField("createdBy");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProdUfieldCore.CreateProductUField(fieldName, createdBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateproductufield")]
        public async Task<ActionResult> UpdateProductUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            try
            {
                if (string.IsNullOrEmpty(oldFieldName))
                {
                    await DataValidator.AddErrorField("oldFieldName");
                }
                if (string.IsNullOrEmpty(newFieldName))
                {
                    await DataValidator.AddErrorField("newFieldName");
                }
                if (string.IsNullOrEmpty(modifiedBy))
                {
                    await DataValidator.AddErrorField("modifiedBy");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProdUfieldCore.UpdateProductUField(oldFieldName, newFieldName, modifiedBy));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteproductufield")]
        public async Task<ActionResult> DeleteProductUField(string fieldName, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    await DataValidator.AddErrorField("fieldName");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProdUfieldCore.DeleteProductUField(fieldName, userAccountId));
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
