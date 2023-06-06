using aims_api.Models;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Interface
{
    public interface IProductConditionCore
    {
        Task<RequestResponse> GetProductConditionPg(int pageNum, int pageItem);
        Task<RequestResponse> GetProductConditionPgSrch(string searchKey, int pageNum, int pageItem);
        Task<RequestResponse> GetProductConditionById(string productConditionId);
        Task<RequestResponse> CreateProductCondition(ProductConditionModel productCondition);
        Task<RequestResponse> UpdateProductCondition(ProductConditionModel productCondition);
        Task<RequestResponse> DeleteProductCondition(string productConditionId, string userAccountId);
    }
}
