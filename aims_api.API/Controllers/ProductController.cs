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

    public class ProductController : ControllerBase
    {
        private IProductCore ProductCore { get; set; }
        DataValidator DataValidator;

        public ProductController(IProductCore productCore)
        {
            ProductCore = productCore;
            DataValidator = new DataValidator();
        }

        [HttpPost("getproductspecial")]
        public async Task<ActionResult> GetProductSpecial(ProductFilterMdl filter, string? searchKey, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCore.GetProductSpecial(filter, searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductpg")]
        public async Task<ActionResult> GetProductPg(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCore.GetProductPg(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductpaged")]
        public async Task<ActionResult> GetProductPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCore.GetProductPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductdynpaged")]
        public async Task<ActionResult> GetProductDynPaged(int pageNum = 1, int pageItem = 100)
        {
            try
            {
                return Ok(await ProductCore.GetProductDynPaged(pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductpgsrch")]
        public async Task<ActionResult> GetProductPgSrch(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ProductCore.GetProductPgSrch(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductdynsrchpages")]
        public async Task<ActionResult> GetProductDynSrchPages(string searchKey, int pageNum = 1, int pageItem = 100)
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

                return Ok(await ProductCore.GetProductDynSrchPages(searchKey, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductpgfilterpaged")]
        public async Task<ActionResult> GetProductPgFilteredPaged(ProductFilterMdl filter, int pageNum = 1, int pageItem = 100)
        {
            try
            {
                if (filter == null)
                {
                    await DataValidator.AddErrorField("filter");
                }
                if (filter != null)
                {
                    if (string.IsNullOrEmpty(filter.UomRef) &&
                        string.IsNullOrEmpty(filter.ProductCategoryId) &&
                        string.IsNullOrEmpty(filter.ProductCategoryId2) &&
                        string.IsNullOrEmpty(filter.ProductCategoryId3))
                    {
                        return Ok(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data"));
                    }
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCore.GetProductPgFilteredPaged(filter, pageNum, pageItem));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductbyid")]
        public async Task<ActionResult> GetProductById(string sku)
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

                return Ok(await ProductCore.GetProductById(sku));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpGet("getproductbyidmod")]
        public async Task<ActionResult> GetProductByIdMod(string sku)
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

                return Ok(await ProductCore.GetProductByIdMod(sku));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("createproductmod")]
        public async Task<ActionResult> CreateProduct(ProductModelMod data)
        {
            try
            {
                if (data.Product == null)
                {
                    await DataValidator.AddErrorField("product");
                }
                if ((object?)data.ProdUfields == null)
                {
                    await DataValidator.AddErrorField("prodUFields");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCore.CreateProductMod(data));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpPost("updateproductmod")]
        public async Task<ActionResult> UpdateProduct(ProductModelMod data)
        {
            try
            {
                if (data.Product == null)
                {
                    await DataValidator.AddErrorField("product");
                }
                if ((object?)data.ProdUfields == null)
                {
                    await DataValidator.AddErrorField("prodUFields");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCore.UpdateProductMod(data));
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"ERR500: {ex.Message} @{HttpContext.Request.Host} {ex.StackTrace}");
                return StatusCode(500, new RequestResponse(ResponseCode.FAILED, ex.Message));
                throw;
            }
        }

        [HttpDelete("deleteproduct")]
        public async Task<ActionResult> DeleteProduct(string sku, string userAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(sku))
                {
                    await DataValidator.AddErrorField("sku");
                }
                if (string.IsNullOrEmpty(userAccountId))
                {
                    await DataValidator.AddErrorField("userAccountId");
                }
                if (DataValidator.Invalid)
                {
                    return BadRequest(new RequestResponse(ResponseCode.FAILED, "Invalid Request Data", DataValidator.ErrorFields));
                }

                return Ok(await ProductCore.DeleteProduct(sku, userAccountId));
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
