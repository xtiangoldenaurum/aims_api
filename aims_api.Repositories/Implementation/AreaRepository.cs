using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace aims_api.Repositories.Implementation
{
    public class AreaRepository : IAreaRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        AreaAudit AuditBuilder;
        IPagingRepository PagingRepo;

        public AreaRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new AreaAudit();
            PagingRepo = new PagingRepository();
        }

        public async Task<IEnumerable<AreaModel>> GetAllArea()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from area";
                return await db.QueryAsync<AreaModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<AreaModel>> GetAreaPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from area limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<AreaModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AreaPagedMdl?> GetAreaPaged(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from area limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                
                var ret = await db.QueryAsync<AreaModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = "select count(areaId) from area";
                    var pageDetail = await PagingRepo.GetPageDetail(db, strPgQry, pageNum, pageItem, ret.Count());

                    return new AreaPagedMdl()
                    {
                        Pagination = pageDetail,
                        Area = ret
                    };
                }
            }

            return null;
        }

        public async Task<IEnumerable<AreaModel>> GetAreaPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from area where 
                                                        AreaId like @searchKey or 
                                                        AreaName like @searchKey or 
                                                        Description like @searchKey or 
                                                        DateCreated like @searchKey or 
                                                        DateModified like @searchKey or 
                                                        CreatedBy like @searchKey or 
                                                        ModifiedBy like @searchKey 
                                                        limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<AreaModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<AreaPagedMdl?> GetAreaSrchPaged(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from area ";
                string strFltr = @"where AreaId like @searchKey or 
                                        AreaName like @searchKey or 
                                        Description like @searchKey or 
                                        DateCreated like @searchKey or 
                                        DateModified like @searchKey or 
                                        CreatedBy like @searchKey or 
                                        ModifiedBy like @searchKey";

                strQry += $"{strFltr} limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                
                var ret = await db.QueryAsync<AreaModel>(strQry, param, commandType: CommandType.Text);

                if (ret != null && ret.Any())
                {
                    // build pagination detail
                    string strPgQry = $"select count(areaId) from area {strFltr}";
                    var pgParam = new DynamicParameters();
                    pgParam.Add("@searchKey", $"%{searchKey}%");

                    var pageDeatil = await PagingRepo.GetPageDetail(db, strPgQry, pgParam, pageNum, pageItem, ret.Count());

                    return new AreaPagedMdl()
                    {
                        Pagination = pageDeatil,
                        Area = ret
                    };
                }
            }

            return null;
        }

        public async Task<AreaModel> GetAreaById(string areaId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from area where areaId = @areaId";

                var param = new DynamicParameters();
                param.Add("@areaId", areaId);
                return await db.QuerySingleOrDefaultAsync<AreaModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> AreaExists(string areaId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select areaId from 
                                            area where 
                                                areaId = @areaId";

                var param = new DynamicParameters();
                param.Add("@areaId", areaId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateArea(AreaModel area)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"insert into area(areaId, 
                                                areaName, 
                                                description, 
                                                createdBy, 
                                                modifiedBy) 
                                            values(@areaId, 
                                                    @areaName, 
                                                    @description, 
                                                    @createdBy, 
                                                    @modifiedBy)";

                int res = await db.ExecuteAsync(strQry, area);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(area);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateArea(AreaModel area)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"update area set 
                                                areaName = @areaName, 
                                                description = @description, 
                                                modifiedBy = @modifiedBy where 
                                                areaId = @areaId";

                int res = await db.ExecuteAsync(strQry, area);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(area);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> AreaInUse(string areaId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select count(areaId) from 
                                            location where 
                                                areaId = @areaId";

                var param = new DynamicParameters();
                param.Add("@areaId", areaId);

                var res = await db.ExecuteScalarAsync<int>(strQry, param);
                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteArea(string areaId, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                var area = await GetAreaById(areaId);

                if (area != null)
                {
                    string strQry = @"delete from area where areaId = @areaId";

                    var param = new DynamicParameters();
                    param.Add("@areaId", areaId);

                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(area, userAccountId);

                        if (await AuditTrailRepo.CreateAuditTrail(db, audit))
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
