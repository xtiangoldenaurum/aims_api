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
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        ProductCategoryAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public ProductCategoryRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ProductCategoryAudit();
            PagingRepo = new PagingRepository();
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetProductCategoryPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ProductCategory limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ProductCategoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ProductCategoryPagedMdl?> GetProductCategoryPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from ProductCategory limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                
                var ret = await db.QueryAsync<ProductCategoryModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(productCategoryId) from productcategory";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new ProductCategoryPagedMdl()
                    {
                        Pagination = pageDetail,
                        ProductCategory = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetProductCategoryPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ProductCategory where 
														productCategoryId like @searchKey or 
														productCategory like @searchKey or 
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
                return await db.QueryAsync<ProductCategoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ProductCategoryPagedMdl?> GetProductCategorySrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ProductCategory ";

                string strFltr = @"where productCategoryId like @searchKey or 
										    productCategory like @searchKey or 
										    description like @searchKey or 
										    dateCreated like @searchKey or 
										    dateModified like @searchKey or 
										    createdBy like @searchKey or 
										    modifiedBy like @searchKey";

                strQry += $"{strFltr} limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                
                var ret = await db.QueryAsync<ProductCategoryModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(productCategoryId) from ProductCategory {strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDeatil = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new ProductCategoryPagedMdl()
                    {
                        Pagination = pageDeatil,
                        ProductCategory = ret
                    };
                }
            }

            return null;
        }

        public async Task<ProductCategoryModel> GetProductCategoryById(string productCategoryId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from ProductCategory where 
														productCategoryId = @productCategoryId";

                var param = new DynamicParameters();
				param.Add("@productCategoryId", productCategoryId);
                return await db.QuerySingleOrDefaultAsync<ProductCategoryModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ProductCategoryExists(string productCategoryId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select productCategoryId from ProductCategory where 
														productCategoryId = @productCategoryId";

                var param = new DynamicParameters();
				param.Add("@productCategoryId", productCategoryId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateProductCategory(ProductCategoryModel productCategory)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"insert into ProductCategory(productCategoryId, 
														productCategory, 
														description, 
														createdBy, 
														modifiedBy)
 												values(@productCategoryId, 
														@productCategory, 
														@description, 
														@createdBy, 
														@modifiedBy)";

                int res = await db.ExecuteAsync(strQry, productCategory);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(productCategory);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateProductCategory(ProductCategoryModel productCategory)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"update ProductCategory set 
														productCategory = @productCategory, 
														description = @description, 
														modifiedBy = @modifiedBy where 
														productCategoryId = @productCategoryId";

                int res = await db.ExecuteAsync(strQry, productCategory);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(productCategory);

                    if (await AuditTrailRepo.CreateAuditTrail(audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function
        public async Task<bool> ProductCategoryInUse(string productCategoryId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT COUNT(sku) AS timesUse 
                                    FROM product 
                                    WHERE ProductCategoryId = @productCategoryId OR 
                                            ProductCategoryId = @productCategoryId OR 
                                            ProductCategoryId = @productCategoryId;";

                var param = new DynamicParameters();
                param.Add("@productCategoryId", productCategoryId);
                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteProductCategory(string productCategoryId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from ProductCategory where 
														productCategoryId = @productCategoryId";
                var param = new DynamicParameters();
				param.Add("@productCategoryId", productCategoryId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
