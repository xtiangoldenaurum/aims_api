using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class ProductRepository : IProductRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        IProductPricingRepository ProductPricingRepo;
        IProductUserFieldRepository ProdUFieldRepo;
        ProductAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public ProductRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo, IProductPricingRepository prodPricingRepo, IProductUserFieldRepository prodFieldRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            ProductPricingRepo = prodPricingRepo;
            ProdUFieldRepo = prodFieldRepo;
            PagingRepo = new PagingRepository();
            AuditBuilder = new ProductAudit();
        }

        public async Task<IEnumerable<ProductModel>> GetProductPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Product limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ProductModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ProductPagedMdl?> GetProductPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Product order by productName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);


                var ret = await db.QueryAsync<ProductModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    string strPgQry = @"select count(sku) from product;";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new ProductPagedMdl()
                    {
                        Pagination = pageDetail,
                        Product = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<dynamic>> GetProductPgDyn(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "SELECT prod.*, prodUF.* FROM product prod INNER JOIN productuserfield prodUF ON prod.sku = prodUF.sku limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<dynamic>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ProductDynPagedMdl?> GetProductDynPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT prod.*, 
                                            prodUF.* 
                                    FROM product prod INNER JOIN 
                                            productuserfield prodUF ON 
                                            prod.sku = prodUF.sku 
                                    order by prod.sku 
                                    limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<dynamic>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = @"SELECT count(prod.sku) FROM product prod INNER JOIN 
                                            productuserfield prodUF ON
                                            prod.sku = prodUF.sku";

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new ProductDynPagedMdl()
                    {
                        Pagination = pageDetail,
                        Product = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<ProductModel>> GetProductPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Product where 
														sku like @searchKey or 
														productName like @searchKey or 
														description like @searchKey or 
														barcode like @searchKey or 
														barcode2 like @searchKey or 
														barcode3 like @searchKey or 
														barcode4 like @searchKey or 
														uniqueRfid like @searchKey or 
														qrCode like @searchKey or 
														uomRef like @searchKey or 
														length like @searchKey or 
														width like @searchKey or 
														height like @searchKey or 
														cubic like @searchKey or 
														grossWeight like @searchKey or 
														netWeight like @searchKey or 
														productCategoryId like @searchKey or 
														productCategoryId2 like @searchKey or 
														productCategoryId3 like @searchKey or 
														image like @searchKey or 
														captureTag like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<ProductModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<ProductDynPagedMdl?> GetProductDynSrchPages(string searchKey, int pageNum, int pageItem)
        {
            // get product User Fields filter qry
            string? strUFQry = await ProdUFieldRepo.GetProdUsrFldQryFilter();

            // pagination setup
            int offset = (pageNum - 1) * pageItem;

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT prod.*, 
                                            prodUF.* 
                                    FROM product prod INNER JOIN 
                                            productuserfield prodUF ON 
                                            prod.sku = prodUF.sku ";

                string strFltr = @"where prod.sku like @searchKey or 
									        prod.productName like @searchKey or 
									        prod.description like @searchKey or 
									        prod.barcode like @searchKey or 
									        prod.barcode2 like @searchKey or 
									        prod.barcode3 like @searchKey or 
									        prod.barcode4 like @searchKey or 
									        prod.uniqueRfid like @searchKey or 
									        prod.qrCode like @searchKey or 
									        prod.uomRef like @searchKey or 
									        prod.length like @searchKey or 
									        prod.width like @searchKey or 
									        prod.height like @searchKey or 
									        prod.cubic like @searchKey or 
									        prod.grossWeight like @searchKey or 
									        prod.netWeight like @searchKey or 
									        prod.productCategoryId like @searchKey or 
									        prod.productCategoryId2 like @searchKey or 
									        prod.productCategoryId3 like @searchKey or 
									        prod.captureTag like @searchKey or 
									        prod.dateCreated like @searchKey or 
									        prod.dateModified like @searchKey or 
									        prod.createdBy like @searchKey or 
									        prod.modifiedBy like @searchKey ";

                if (strUFQry != null && strUFQry.Length > 0)
                {
                    strFltr += $"or {strUFQry}";
                }
                strQry += $"{strFltr} order by prod.sku limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<dynamic>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $@"SELECT count(prod.sku) 
                                        FROM product prod INNER JOIN
                                                productuserfield prodUF ON
                                                prod.sku = prodUF.sku {strFltr}";

                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new ProductDynPagedMdl()
                    {
                        Pagination = pageDetail,
                        Product = ret
                    };

                }
            }

            return null;
        }

        public async Task<ProductDynPagedMdl?> GetProductPgFilteredPaged(ProductFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = @"SELECT prod.*, 
                                        prodUF.* 
                                FROM product prod INNER JOIN 
                                        productuserfield prodUF ON 
                                        prod.sku = prodUF.sku ";

            string strFltr = " where ";
            var param = new DynamicParameters();
            var pgParam = new DynamicParameters();

            // build filter query
            if (!string.IsNullOrEmpty(filter.UomRef))
            {
                strFltr += $"prod.uomRef = @uomRef ";
                param.Add("@uomRef", filter.UomRef);
            }

            if (!string.IsNullOrEmpty(filter.ProductCategoryId))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"prod.productCategoryId = @productCategoryId";
                param.Add("@productCategoryId", filter.ProductCategoryId);
            }

            if (!string.IsNullOrEmpty(filter.ProductCategoryId2))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"prod.productCategoryId2 = @productCategoryId2";
                param.Add("@productCategoryId2", filter.ProductCategoryId2);
            }

            if (!string.IsNullOrEmpty(filter.ProductCategoryId3))
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"prod.productCategoryId3 = @productCategoryId3";
                param.Add("@productCategoryId3", filter.ProductCategoryId3);
            }

            // build order by and paging
            strQry += strFltr + $" order by prod.sku limit @pageItem offset @offset";

            // set paging parameter
            pgParam = param;

            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<dynamic>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $@"SELECT count(prod.sku) 
                                            FROM product prod 
                                            INNER JOIN productuserfield prodUF 
                                            ON prod.sku = prodUF.sku {strFltr}";

                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new ProductDynPagedMdl()
                    {
                        Pagination = pageDetail,
                        Product = ret
                    };
                }
            }

            return null;
        }

        public async Task<ProductModel> GetProductById(string sku)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Product where 
														sku = @sku";

                var param = new DynamicParameters();
                param.Add("@sku", sku);
                return await db.QuerySingleOrDefaultAsync<ProductModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ProductExists(string sku)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select sku from Product where 
														sku = @sku";

                var param = new DynamicParameters();
                param.Add("@sku", sku);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateProductMod(string sku, string createdBy, ProductModelMod data)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var prodCreated = await CreateProduct(db, data.Product);

                if (prodCreated)
                {
                    // init productuserfield default data
                    var initProdUFld = await ProdUFieldRepo.InitProdUField(db, sku);
                    if (initProdUFld)
                    {
                        // record product userfield data
                        var uFieldsCreated = await ProdUFieldRepo.UpdateProdUField(db, sku, createdBy, data.ProdUfields);
                        if (uFieldsCreated)
                        {
                            // build product pricing in case null
                            var prodPricing = data.ProductPricing == null ? new ProductPricingModel { SKU = data.Product.Sku } : data.ProductPricing;

                            // record product pricing
                            var pricingCreated = await ProductPricingRepo.CreateProductPricingMod(db, prodPricing, data.Product.CreatedBy);
                            if (pricingCreated)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> CreateProduct(IDbConnection db, ProductModel product)
        {
            string strQry = @"insert into Product(sku, 
														productName, 
														description, 
														barcode, 
														barcode2, 
														barcode3, 
														barcode4, 
														uniqueRfid, 
														qrCode, 
														uomRef, 
														length, 
														width, 
														height, 
														cubic, 
														grossWeight, 
														netWeight, 
														productCategoryId, 
														productCategoryId2, 
														productCategoryId3, 
														image, 
														captureTag, 
														createdBy, 
														modifiedBy)
 												values(@sku, 
														@productName, 
														@description, 
														@barcode, 
														@barcode2, 
														@barcode3, 
														@barcode4, 
														@uniqueRfid, 
														@qrCode, 
														@uomRef, 
														@length, 
														@width, 
														@height, 
														@cubic, 
														@grossWeight, 
														@netWeight, 
														@productCategoryId, 
														@productCategoryId2, 
														@productCategoryId3, 
														@image, 
														@captureTag, 
														@createdBy, 
														@modifiedBy)";

            int res = await db.ExecuteAsync(strQry, product);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditADD(product);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateProductMod(string sku, string modifiedBy, ProductModelMod data)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                var prodUpdated = await UpdateProduct(db, data.Product);

                if (prodUpdated)
                {
                    // update product user field
                    var uFieldsCreated = await ProdUFieldRepo.UpdateProdUFieldMOD(db, sku, modifiedBy, data.ProdUfields);
                    if (uFieldsCreated)
                    {
                        // update product pricing
                        var pricingUpdated = await ProductPricingRepo.UpdateProductPricingMod(db, data.ProductPricing, data.Product.CreatedBy);
                        if (pricingUpdated)
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateProduct(IDbConnection db, ProductModel product)
        {
            string strQry = @"update Product set 
														productName = @productName, 
														description = @description, 
														barcode = @barcode, 
														barcode2 = @barcode2, 
														barcode3 = @barcode3, 
														barcode4 = @barcode4, 
														uniqueRfid = @uniqueRfid, 
														qrCode = @qrCode, 
														uomRef = @uomRef, 
														length = @length, 
														width = @width, 
														height = @height, 
														cubic = @cubic, 
														grossWeight = @grossWeight, 
														netWeight = @netWeight, 
														productCategoryId = @productCategoryId, 
														productCategoryId2 = @productCategoryId2, 
														productCategoryId3 = @productCategoryId3, 
														image = @image, 
														captureTag = @captureTag, 
														modifiedBy = @modifiedBy where 
														sku = @sku";

            int res = await db.ExecuteAsync(strQry, product);

            if (res > 0)
            {
                var audit = await AuditBuilder.BuildTranAuditMOD(product);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }

        // place here InUse checker function
        public async Task<bool> ProductInUse(string sku)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT SUM(
				                                (select count(distinct @sku) from inventory) + 
				                                (select count(distinct @sku) from podetail) + 
				                                (select count(distinct @sku) from returnsdetail) + 
				                                (select count(distinct @sku) from runningbalance) + 
				                                (select count(distinct @sku) from sodetail) + 
				                                (select count(distinct @sku) from uniquetags) + 
				                                (select count(distinct @sku) from whtransferdetail) + 
                                                (select count(distinct @sku) from sodetail)
                                ) AS timesUsed";

                var param = new DynamicParameters();
                param.Add("@sku", sku);
                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteProduct(string sku, string userAccountId)
        {
            // init podUserField audit builder
            var prodUFldAudit = new ProductUserFieldAudit();

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                // get product detail
                var product = await GetProductById(sku);

                // get productUserField detail
                var prodUFields = await ProdUFieldRepo.GetProductUFieldById(sku);

                if (product != null && prodUFields != null)
                {
                    string strQry = @"delete from Product where 
														sku = @sku";
                    var param = new DynamicParameters();
                    param.Add("@sku", sku);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(product, userAccountId);
                        var auditDtl = await prodUFldAudit.BuildTranAuditDEL(sku, userAccountId, prodUFields);

                        var headAudit = await AuditTrailRepo.CreateAuditTrail(db, audit);
                        var dtlAudit = await AuditTrailRepo.CreateAuditTrail(db, auditDtl);

                        if (headAudit && dtlAudit)
                        {
                            tran.Commit();
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }
}
