using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace aims_api.Repositories.Implementation
{
    public class ProductPricingRepository : IProductPricingRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        ProductPricingAudit AuditBuilder;

        public ProductPricingRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ProductPricingAudit();
        }

        public async Task<ProductPricingModel> GetPricingBySKU(string sku)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from productpricing where sku = @sku";

                var param = new DynamicParameters();
                param.Add("@sku", sku);
                return await db.QuerySingleOrDefaultAsync<ProductPricingModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> CreateProductPricingMod(IDbConnection db, 
                                                        ProductPricingModel prodPricing, 
                                                        string userAccountId)
        {

            string strQry = @"insert into productpricing(sku, 
                                                            cost, 
                                                            retailPrice, 
                                                            wholeSalePrice, 
                                                            discountedPrice) 
                                                        values(@sku, 
                                                                @cost, 
                                                                @retailPrice, 
                                                                @wholeSalePrice, 
                                                                @discountedPrice)";

            int res = await db.ExecuteAsync(strQry, prodPricing);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildProdPriceAuditADD(prodPricing, userAccountId);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateProductPricingMod(IDbConnection db, 
                                                        ProductPricingModel prodPricing, 
                                                        string userAccountId)
        {

            string strQry = @"update productpricing set cost = @cost, 
                                                        retailPrice = @retailPrice, 
                                                        wholeSalePrice = @wholeSalePrice, 
                                                        discountedPrice = @discountedPrice 
                                where sku = @sku";

            int res = await db.ExecuteAsync(strQry, prodPricing);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildProdPriceAuditMOD(prodPricing, userAccountId);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
