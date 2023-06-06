using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetProductPg(int pageNum, int pageItem);
        Task<ProductPagedMdl?> GetProductPaged(int pageNum, int pageItem);
        Task<IEnumerable<dynamic>> GetProductPgDyn(int pageNum, int pageItem);
        Task<ProductDynPagedMdl?> GetProductDynPaged(int pageNum, int pageItem);
        Task<IEnumerable<ProductModel>> GetProductPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ProductDynPagedMdl?> GetProductDynSrchPages(string searchKey, int pageNum, int pageItem);
        Task<ProductDynPagedMdl?> GetProductPgFilteredPaged(ProductFilterMdl filter, int pageNum, int pageItem);
        Task<ProductModel> GetProductById(string sku);
        Task<bool> ProductExists(string sku);
        Task<bool> CreateProductMod(string sku, string createdBy, ProductModelMod data);
        Task<bool> CreateProduct(IDbConnection db, ProductModel product);
        Task<bool> UpdateProductMod(string sku, string modifiedBy, ProductModelMod data);
        Task<bool> UpdateProduct(IDbConnection db, ProductModel product);
        Task<bool> ProductInUse(string sku);
        Task<bool> DeleteProduct(string sku, string userAccountId);
    }
}
