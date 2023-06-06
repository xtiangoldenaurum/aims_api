using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class ProductCore : IProductCore
    {
        private IProductRepository ProductRepo { get; set; }
        private IProductPricingRepository ProductPricingRepo { get; set; }
        private IProductUserFieldRepository ProdUFieldRepo { get; set; }
        public ProductCore(IProductRepository productRepo, IProductPricingRepository productPricingRepo, IProductUserFieldRepository prodUFieldRepo)
        {
            ProductRepo = productRepo;
            ProductPricingRepo = productPricingRepo;
            ProdUFieldRepo = prodUFieldRepo;
        }

        public async Task<RequestResponse> GetProductSpecial(ProductFilterMdl filter, string? searchKey, int pageNum, int pageItem)
        {
            ProductDynPagedMdl? data = null;
            bool skip = false;

            // do filtered query
            if (!string.IsNullOrEmpty(filter.UomRef) ||
                !string.IsNullOrEmpty(filter.ProductCategoryId) ||
                !string.IsNullOrEmpty(filter.ProductCategoryId2) ||
                !string.IsNullOrEmpty(filter.ProductCategoryId3))
            {
                data = await ProductRepo.GetProductPgFilteredPaged(filter, pageNum, pageItem);
                skip = true;
            }

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await ProductRepo.GetProductDynSrchPages(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await ProductRepo.GetProductDynPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductPg(int pageNum, int pageItem)
        {   
            var data = await ProductRepo.GetProductPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductPaged(int pageNum, int pageItem)
        {
            var data = await ProductRepo.GetProductPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductDynPaged(int pageNum, int pageItem)
        {
            var data = await ProductRepo.GetProductDynPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ProductRepo.GetProductPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductDynSrchPages(string searchKey, int pageNum, int pageItem)
        {
            var data = await ProductRepo.GetProductDynSrchPages(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductPgFilteredPaged(ProductFilterMdl filter, int pageNum, int pageItem)
        {
            var data = await ProductRepo.GetProductPgFilteredPaged(filter, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductById(string sku)
        {
            var data = await ProductRepo.GetProductById(sku);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductByIdMod(string sku)
        {
            var data = new ProductModelMod();

            var product = await ProductRepo.GetProductById(sku);
            var pricing = await ProductPricingRepo.GetPricingBySKU(sku);
            var prodUFields = await ProdUFieldRepo.GetProductUFieldById(sku);

            if (prodUFields == null)
            {
                // create empty nulled columns incase there's no user fields values
                prodUFields = await ProdUFieldRepo.GetProductUFields();
            }

            if (product != null)
            {
                data.Product = product;
                data.ProductPricing = pricing;
                data.ProdUfields = prodUFields;

                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateProductMod(ProductModelMod data)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string? sku = data.Product.Sku;
            string? createdBy = data.Product.CreatedBy;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            bool productExists = await ProductRepo.ProductExists(sku);

            if (productExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar Sku exists.");
            }

            bool res = await ProductRepo.CreateProductMod(sku, createdBy, data);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateProductMod(ProductModelMod data)
        {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string? sku = data.Product.Sku;
            string? modifiedBy = data.Product.ModifiedBy;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            bool res = await ProductRepo.UpdateProductMod(sku, modifiedBy, data);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteProduct(string sku, string userAccountId)
        {
            // check if product is in use
            var inUse = await ProductRepo.ProductInUse(sku);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. Product SKU is in use.");
            }

            bool res = await ProductRepo.DeleteProduct(sku, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
