using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Interface
{
    public interface IProductPricingRepository
    {
        Task<ProductPricingModel> GetPricingBySKU(string sku);
        Task<bool> CreateProductPricingMod(IDbConnection db, ProductPricingModel prodPricing, string userAccountId);
        Task<bool> UpdateProductPricingMod(IDbConnection db, ProductPricingModel prodPricing, string userAccountId);
    }
}
