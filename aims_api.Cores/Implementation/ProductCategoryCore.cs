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
    public class ProductCategoryCore : IProductCategoryCore
    {
        private IProductCategoryRepository ProductCategoryRepo { get; set; }
        public ProductCategoryCore(IProductCategoryRepository productCategoryRepo)
        {
            ProductCategoryRepo = productCategoryRepo;
        }

        public async Task<RequestResponse> GetProductCatSpecial(string? searchKey, int pageNum, int pageItem)
        {
            ProductCategoryPagedMdl? data = null;
            bool skip = false;

            // do search query
            if (!string.IsNullOrEmpty(searchKey) && !skip)
            {
                data = await ProductCategoryRepo.GetProductCategorySrchPaged(searchKey, pageNum, pageItem);
                skip = true;
            }

            // else do get all query
            if (!skip)
            {
                data = await ProductCategoryRepo.GetProductCategoryPaged(pageNum, pageItem);
            }

            // return result if there is
            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductCategoryPg(int pageNum, int pageItem)
        {   
            var data = await ProductCategoryRepo.GetProductCategoryPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductCategoryPaged(int pageNum, int pageItem)
        {
            var data = await ProductCategoryRepo.GetProductCategoryPaged(pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductCategoryPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ProductCategoryRepo.GetProductCategoryPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductCategorySrchPaged(string searchKey, int pageNum, int pageItem)
        {
            var data = await ProductCategoryRepo.GetProductCategorySrchPaged(searchKey, pageNum, pageItem);

            if (data != null && data.Pagination != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductCategoryById(string productCategoryId)
        {
            var data = await ProductCategoryRepo.GetProductCategoryById(productCategoryId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateProductCategory(ProductCategoryModel productCategory)
        {
            bool productCategoryExists = await ProductCategoryRepo.ProductCategoryExists(productCategory.ProductCategoryId);
            if (productCategoryExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ProductCategoryId exists.");
            }

            bool res = await ProductCategoryRepo.CreateProductCategory(productCategory);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateProductCategory(ProductCategoryModel productCategory)
        {
            bool res = await ProductCategoryRepo.UpdateProductCategory(productCategory);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteProductCategory(string productCategoryId, string userAccountId)
        {
            // check if productcategory is in use
            var inUse = await ProductCategoryRepo.ProductCategoryInUse(productCategoryId);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. Product Category is in use.");
            }

            bool res = await ProductCategoryRepo.DeleteProductCategory(productCategoryId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
