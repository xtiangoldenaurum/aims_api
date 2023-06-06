using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;

namespace aims_api.Cores.Implementation
{
    public class ProductUserFieldCore : IProductUserFieldCore
    {
        private IProductUserFieldRepository ProdUFieldRepo { get; set; }
        private IAuditTrailRepository AuditRepo { get; set; }

        public ProductUserFieldCore(IProductUserFieldRepository prodUFieldRepo, IAuditTrailRepository auditRepo)
        {
            ProdUFieldRepo = prodUFieldRepo;
            AuditRepo = auditRepo;
        }

        public async Task<RequestResponse> GetProductUFieldById(string sku)
        {
            var data = await ProdUFieldRepo.GetProductUFieldById(sku);

            if ((object)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetProductUFields()
        {
            var data = await ProdUFieldRepo.GetProductUFields();

            if ((object?)data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateProductUField(string fieldName, string createdBy)
        {
            // check is column name is in use
            var columnExists = await ProdUFieldRepo.ChkColExists(fieldName);
            if (columnExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar column name exists.");
            }

            var res = await ProdUFieldRepo.CreateProductUField(fieldName, createdBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateProductUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            var res = await ProdUFieldRepo.UpdateProductUField(oldFieldName, newFieldName, modifiedBy);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteProductUField(string fieldName, string userAccountId)
        {
            bool res = await ProdUFieldRepo.DeleteProductUField(fieldName, userAccountId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
