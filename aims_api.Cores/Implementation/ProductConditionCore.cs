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
    public class ProductConditionCore : IProductConditionCore
    {
        private IProductConditionRepository ProductConditionRepo { get; set; }
        public ProductConditionCore(IProductConditionRepository productConditionRepo)
        {
            ProductConditionRepo = productConditionRepo;
        }

        public async Task<RequestResponse> GetProductConditionPg(int pageNum, int pageItem)
        {   
            var data = await ProductConditionRepo.GetProductConditionPg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductConditionPgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await ProductConditionRepo.GetProductConditionPgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductConditionById(string productConditionId)
        {
            var data = await ProductConditionRepo.GetProductConditionById(productConditionId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateProductCondition(ProductConditionModel productCondition)
        {
            bool productConditionExists = await ProductConditionRepo.ProductConditionExists(productCondition.ProductConditionId);
            if (productConditionExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar ProductConditionId exists.");
            }

            bool res = await ProductConditionRepo.CreateProductCondition(productCondition);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateProductCondition(ProductConditionModel productCondition)
        {
            bool res = await ProductConditionRepo.UpdateProductCondition(productCondition);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteProductCondition(string productConditionId, string userAccountId)
        {
            // check if product condition is in use
            var inUse = await ProductConditionRepo.ProductConditionInUse(productConditionId);
            if (inUse)
            {
                return new RequestResponse(ResponseCode.FAILED, "Delete failed. Product Condition is in use.");
            }

            bool res = await ProductConditionRepo.DeleteProductCondition(productConditionId, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
