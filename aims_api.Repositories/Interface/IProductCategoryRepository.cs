using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategoryModel>> GetProductCategoryPg(int pageNum, int pageItem);
        Task<ProductCategoryPagedMdl?> GetProductCategoryPaged(int pageNum, int pageItem);
        Task<IEnumerable<ProductCategoryModel>> GetProductCategoryPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ProductCategoryPagedMdl?> GetProductCategorySrchPaged(string searchKey, int pageNum, int pageItem);
        Task<ProductCategoryModel> GetProductCategoryById(string productCategoryId);
        Task<bool> ProductCategoryExists(string productCategoryId);
        Task<bool> CreateProductCategory(ProductCategoryModel productCategory);
        Task<bool> UpdateProductCategory(ProductCategoryModel productCategory);
        Task<bool> ProductCategoryInUse(string productCategoryId);
        Task<bool> DeleteProductCategory(string productCategoryId, string userAccountId);
    }
}
