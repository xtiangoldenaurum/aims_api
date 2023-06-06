using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IProductConditionRepository
    {
        Task<IEnumerable<ProductConditionModel>> GetProductConditionPg(int pageNum, int pageItem);
        Task<IEnumerable<ProductConditionModel>> GetProductConditionPgSrch(string searchKey, int pageNum, int pageItem);
        Task<ProductConditionModel> GetProductConditionById(string productConditionId);
        Task<bool> ProductConditionExists(string productConditionId);
        Task<bool> CreateProductCondition(ProductConditionModel productCondition);
        Task<bool> UpdateProductCondition(ProductConditionModel productCondition);
        Task<bool> ProductConditionInUse(string productConditionId);
        Task<bool> DeleteProductCondition(string productConditionId, string userAccountId);
    }
}
