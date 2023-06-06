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
using System.Transactions;

namespace aims_api.Repositories.Implementation
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        OrganizationAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public OrganizationRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo, IPagingRepository pagingRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new OrganizationAudit();
            PagingRepo = pagingRepo;
        }

        public async Task<IEnumerable<OrganizationModel>> GetOrganizationPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from Organization limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<OrganizationPagedMdl?> GetOrgPaged(int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = "select * from Organization order by organizationName limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetOrgPageDetail(db, pageNum, pageItem, ret.Count());

                    return new OrganizationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Organization = ret
                    };
                }
            }

            return null;
        }

        public async Task<OrganizationPagedMdl?> GetOrgSearchPaged(string searchKey, int pageNum, int pageItem)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                int offset = (pageNum - 1) * pageItem;
                string strQry = @"select * from Organization where 
														organizationId like @searchKey or 
														organizationName like @searchKey or 
														organizationTypeID like @searchKey or 
														address like @searchKey or 
														address2 like @searchKey or 
														telephone like @searchKey or 
														phone like @searchKey or 
														email like @searchKey or 
														province like @searchKey or 
														city like @searchKey or 
														zipCode like @searchKey or 
														inactive like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey 
                                                        order by organizationname 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                var ret = await db.QueryAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    var pageDetail = await GetOrgSearchPageDetail(db, searchKey, pageNum, pageItem, ret.Count());

                    return new OrganizationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Organization = ret
                    };
                }
            }

            return null;
        }

        public async Task<Pagination?> GetOrgPageDetail(IDbConnection db, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = "select count(organizationId) from organization";
            return await PagingRepo.GetPageDetail(db, strQry, pageNum, pageItem, rowCount);
        }

        public async Task<Pagination?> GetOrgSearchPageDetail(IDbConnection db, string searchKey, int pageNum, int pageItem, int rowCount)
        {
            // provide query here then get page detail from paging repository
            string strQry = @"select count(organizationId) from Organization where 
														organizationId like @searchKey or 
														organizationName like @searchKey or 
														organizationTypeID like @searchKey or 
														address like @searchKey or 
														address2 like @searchKey or 
														telephone like @searchKey or 
														phone like @searchKey or 
														email like @searchKey or 
														province like @searchKey or 
														city like @searchKey or 
														zipCode like @searchKey or 
														inactive like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey";

            var param = new DynamicParameters();
            param.Add("@searchKey", $"%{searchKey}%");

            return await PagingRepo.GetPageDetail(db, strQry, param, pageNum, pageItem, rowCount);
        }

        public async Task<IEnumerable<OrganizationModel>> GetOrganizationPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Organization where 
														organizationId like @searchKey or 
														organizationName like @searchKey or 
														organizationTypeID like @searchKey or 
														address like @searchKey or 
														address2 like @searchKey or 
														telephone like @searchKey or 
														phone like @searchKey or 
														email like @searchKey or 
														province like @searchKey or 
														city like @searchKey or 
														zipCode like @searchKey or 
														inactive like @searchKey or 
														dateCreated like @searchKey or 
														dateModified like @searchKey or 
														createdBy like @searchKey or 
														modifiedBy like @searchKey or 
														remarks like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<OrganizationModel> GetOrganizationById(string organizationId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Organization where 
														organizationId = @organizationId";

                var param = new DynamicParameters();
                param.Add("@organizationId", organizationId);
                return await db.QuerySingleOrDefaultAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> OrganizationExists(string organizationId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select organizationId from Organization where 
														organizationId = @organizationId";

                var param = new DynamicParameters();
                param.Add("@organizationId", organizationId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IEnumerable<OrganizationModel>> GetOrgPgByOrgTypId(string organizationTypeId, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from Organization 
                                    where organizationTypeId = @organizationTypeId 
                                            limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@organizationTypeId", organizationTypeId);
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);

                return await db.QueryAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<LocationModel>> GetOrgPgFiltered(OrganizationFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from organization";
            string strFltr = " where ";
            var param = new DynamicParameters();

            if (!string.IsNullOrEmpty(filter.OrganizationTypeId))
            {
                strFltr += $"organizationTypeId = @organizationTypeId ";
                param.Add("@organizationTypeId", filter.OrganizationTypeId);
            }

            if (filter.Inactive != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"inactive = @inactive ";
                param.Add("@inactive", filter.Inactive);
            }

            // build order by and paging
            strQry += strFltr + $" order by organizationname limit @pageItem offset @offset";

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                return await db.QueryAsync<LocationModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<OrganizationPagedMdl?> GetOrgFilteredPaged(OrganizationFilterMdl filter, int pageNum, int pageItem)
        {
            string strQry = "select * from organization";
            string strFltr = " where ";
            var param = new DynamicParameters();

            // init pagedetail parameters
            var pgParam = new DynamicParameters();
            string strPgQry = "select count(organizationId) from organization";

            if (!string.IsNullOrEmpty(filter.OrganizationTypeId))
            {
                strFltr += $"organizationTypeId = @organizationTypeId ";
                param.Add("@organizationTypeId", filter.OrganizationTypeId);
            }

            if (filter.Inactive != null)
            {
                if (strFltr != " where ")
                {
                    strFltr += " and ";
                }

                strFltr += $"inactive = @inactive ";
                param.Add("@inactive", filter.Inactive);
            }

            // build order by and paging
            strQry += strFltr + $" order by organizationname";
            strQry += $" limit @pageItem offset @offset";

            // set paging param
            pgParam = param;

            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            param.Add("@pageItem", pageItem);
            param.Add("@offset", offset);

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                var ret = await db.QueryAsync<OrganizationModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    strPgQry += strFltr;
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new OrganizationPagedMdl()
                    {
                        Pagination = pageDetail,
                        Organization = ret
                    };
                }
            }

            return null;
        }

        public async Task<bool> CreateOrganization(OrganizationModel organization)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"insert into Organization(organizationId, 
														organizationName, 
														organizationTypeID, 
														address, 
														address2, 
														telephone, 
														phone, 
														email, 
														province, 
														city, 
														zipCode, 
														inactive, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@organizationId, 
														@organizationName, 
														@organizationTypeID, 
														@address, 
														@address2, 
														@telephone, 
														@phone, 
														@email, 
														@province, 
														@city, 
														@zipCode, 
														@inactive, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

                int res = await db.ExecuteAsync(strQry, organization);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(organization);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateOrganization(OrganizationModel organization)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"update Organization set 
														organizationName = @organizationName, 
														organizationTypeID = @organizationTypeID, 
														address = @address, 
														address2 = @address2, 
														telephone = @telephone, 
														phone = @phone, 
														email = @email, 
														province = @province, 
														city = @city, 
														zipCode = @zipCode, 
														inactive = @inactive, 
														modifiedBy = @modifiedBy, 
														remarks = @remarks where 
														organizationId = @organizationId";

                int res = await db.ExecuteAsync(strQry, organization);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(organization);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        // place here InUse checker function
        public async Task<bool> OrganizationInUse(string organizationId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"select count(poId) from po 
                                    where supplierId = @organizationId or 
                                            carrierId = @organizationId";

                var param = new DynamicParameters();
                param.Add("@organizationId", organizationId);
                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteOrganization(string organizationId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                // retreive date for history tracking
                var organization = await GetOrganizationById(organizationId);

                if (organization != null)
                {
                    string strQry = @"delete from Organization where 
														organizationId = @organizationId";
                    var param = new DynamicParameters();
                    param.Add("@organizationId", organizationId);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(organization, userAccountId);

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
