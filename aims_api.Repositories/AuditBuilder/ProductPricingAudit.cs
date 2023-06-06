using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class ProductPricingAudit
    {
        public async Task<AuditTrailModel> BuildProdPriceAuditADD(ProductPricingModel prodPricing, string userAccountId)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(prodPricing);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"{prodPricing.SKU}",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildProdPriceAuditMOD(ProductPricingModel prodPricing, string userAccountId)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(prodPricing);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"{prodPricing.SKU}",
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildProdPriceAuditDEL(ProductPricingModel prodPricing, string userAccountId)
        {
            string actTyp = "DEL";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(prodPricing);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = $"{prodPricing.SKU}",
                    Data = data
                };
            });
        }
    }
}
