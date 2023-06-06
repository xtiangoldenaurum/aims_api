using aims_api.Models;
using aims_api.Utilities;

namespace aims_api.Cores.Interface
{
    public interface IProductUserFieldCore
    {
        Task<RequestResponse> GetProductUFieldById(string sku);
        Task<RequestResponse> GetProductUFields();
        Task<RequestResponse> CreateProductUField(string fieldName, string createdBy);
        Task<RequestResponse> UpdateProductUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<RequestResponse> DeleteProductUField(string fieldName, string userAccountId);
    }
}
