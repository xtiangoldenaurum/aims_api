using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IProductCore
    {
        Task<RequestResponse> GetProductSpecial(ProductFilterMdl filter, string? searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductPg(int pageNum, int pageItem);
        Task<RequestResponse> GetProductPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetProductDynPaged(int pageNum, int pageItem);
        Task<RequestResponse> GetProductPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductDynSrchPages(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductPgFilteredPaged(ProductFilterMdl filter, int pageNum, int pageItem);
        Task<RequestResponse> GetProductById(string sku);
        Task<RequestResponse> GetProductByIdMod(string sku);
        Task<RequestResponse> CreateProductMod(ProductModelMod data);
        Task<RequestResponse> UpdateProductMod(ProductModelMod product);
        Task<RequestResponse> DeleteProduct(string sku, string userAccountId);
    }
}
