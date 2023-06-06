using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class ProductConditionRepository : IProductConditionRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        ProductConditionAudit AuditBuilder;

        public ProductConditionRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ProductConditionAudit();
        }

        public async Task<IEnumerable<ProductConditionModel>> GetProductConditionPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ProductCondition limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ProductConditionModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ProductConditionModel>> GetProductConditionPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ProductCondition where 
														productConditionId like @searchKey or 
														productCondition like @searchKey or 
														description like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ProductConditionModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ProductConditionModel> GetProductConditionById(string productConditionId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ProductCondition where 
														productConditionId = @productConditionId";

                var param = new DynamicParameters();
				param.Add("@productConditionId", productConditionId);
                return await db.QuerySingleOrDefaultAsync<ProductConditionModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ProductConditionExists(string productConditionId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select productConditionId from ProductCondition where 
														productConditionId = @productConditionId";

                var param = new DynamicParameters();
				param.Add("@productConditionId", productConditionId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateProductCondition(ProductConditionModel productCondition)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"insert into ProductCondition(productConditionId, 
														productCondition, 
														description, 
														createdBy, 
														modifiedBy)
 												values(@productConditionId, 
														@productCondition, 
														@description, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, productCondition);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(productCondition);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateProductCondition(ProductConditionModel productCondition)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update ProductCondition set 
														productCondition = @productCondition, 
														description = @description, 
														modifiedBy = @modifiedBy where 
														productConditionId = @productConditionId";

                int res = await db.ExecuteAsync(strQry, productCondition);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(productCondition);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function
        public async Task<bool> ProductConditionInUse(string productConditionId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT COUNT(lotAttributeId) 
                                    FROM lotattributedetail 
                                    WHERE productConditionId = @productConditionId";

                var param = new DynamicParameters();
                param.Add("@productConditionId", productConditionId);
                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteProductCondition(string productConditionId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // get product condtion detail
                var productCondition = await GetProductConditionById(productConditionId);

                if (productCondition != null)
                {
                    string strQry = @"delete from ProductCondition where 
														productConditionId = @productConditionId";
                    var param = new DynamicParameters();
                    param.Add("@productConditionId", productConditionId);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(productCondition, userAccountId);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
