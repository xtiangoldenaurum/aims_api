using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IProductUserFieldRepository
    {
        Task<dynamic> GetProductUFieldById(string sku);
        Task<dynamic?> GetProductUFields();
        Task<string?> GetProdUsrFldQryFilter();
        Task<bool> CreateProductUField(string fieldName, string createdBy);
        Task<bool> UpdateProdUField(dynamic data);
        Task<bool> InitProdUField(IDbConnection db, string sku);
        Task<bool> UpdateProdUField(IDbConnection db, string sku, string createdBy, dynamic data);
        Task<bool> UpdateProdUFieldMOD(IDbConnection db, string sku, string modifiedBy, dynamic data);
        Task<bool> ChkColExists(string fieldName);
        Task<bool> UpdateProductUField(string oldFieldName, string newFieldName, string modifiedBy);
        Task<bool> DeleteProductUField(string fieldName, string userAccountId);
    }
}
