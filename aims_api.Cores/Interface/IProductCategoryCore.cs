using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IProductCategoryCore
    {
        Task<RequestResponse> GetProductCatSpecial(string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductCategoryPg(int pageNum, int pageItem);
        Task<RequestResponse> GetProductCategoryPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetProductCategoryPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductCategorySrchPaged(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductCategoryById(string productCategoryId);
        Task<RequestResponse> CreateProductCategory(ProductCategoryModel productCategory);
        Task<RequestResponse> UpdateProductCategory(ProductCategoryModel productCategory);
        Task<RequestResponse> DeleteProductCategory(string productCategoryId, string userAccountId);
    }
}
