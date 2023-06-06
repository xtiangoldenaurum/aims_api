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

    public class ProductCategoryController : ControllerBase
    {
        private IProductCategoryCore ProductCategoryCore { get; set; }
        DataValidator DataValidator;

        public ProductCategoryController(IProductCategoryCore productCategoryCore)
        {
            ProductCategoryCore = productCategoryCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getproductcatspecial")]
        public async Task<ActionResult> GetProductCatSpecial(string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCategoryCore.GetProductCatSpecial(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductcategorypg")]
        public async Task<ActionResult> GetProductCategoryPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCategoryCore.GetProductCategoryPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductcategorypaged")]
        public async Task<ActionResult> GetProductCategoryPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCategoryCore.GetProductCategoryPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductcategorypgsrch")]
        public async Task<ActionResult> GetProductCategoryPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ProductCategoryCore.GetProductCategoryPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductcategorysrchpaged")]
        public async Task<ActionResult> GetProductCategorySrchPaged(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ProductCategoryCore.GetProductCategorySrchPaged(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductcategorybyid")]
        public async Task<ActionResult> GetProductCategoryById(string productCategoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(productCategoryId))
                {
                    await DataValidator.AddErrorField("productCategoryId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCategoryCore.GetProductCategoryById(productCategoryId));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createproductcategory")]
        public async Task<ActionResult> CreateProductCategory(ProductCategoryModel productCategory)
        {
            try
            {
                if (productCategory == null)
                {
                    await DataValidator.AddErrorField("productCategory");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCategoryCore.CreateProductCategory(productCategory));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateproductcategory")]
        public async Task<ActionResult> UpdateProductCategory(ProductCategoryModel productCategory)
        {
            try
            {
                if (productCategory == null)
                {
                    await DataValidator.AddErrorField("productCategory");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCategoryCore.UpdateProductCategory(productCategory));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteproductcategory")]
        public async Task<ActionResult> DeleteProductCategory(string productCategoryId, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(productCategoryId))
                {
                    await DataValidator.AddErrorField("productCategoryId");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCategoryCore.DeleteProductCategory(productCategoryId, userAccountId));
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
