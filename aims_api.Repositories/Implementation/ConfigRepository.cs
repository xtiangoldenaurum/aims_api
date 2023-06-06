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
    public class ConfigRepository : IConfigRepository
    {
        private string ConnString;
        IAuditTrailRepository AuditTrailRepo;
        ConfigAudit AuditBuilder;

        public ConfigRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ConfigAudit();
        }

        public async Task<IEnumerable<ConfigModel>> GetAllConfig()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from config";
                return await db.QueryAsync<ConfigModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ConfigModel>> GetConfigPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from config limit @pageItem offset @offset";
                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("offset", offset);
                return await db.QueryAsync<ConfigModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<ConfigModel>> GetConfigPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;

            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from config where 
                                                        ConfigType like @searchKey or 
                                                        ConfigName like @searchKey or 
                                                        Description like @searchKey or 
                                                        StringValue like @searchKey or 
                                                        IntValue like @searchKey or 
                                                        DateFrom like @searchKey or 
                                                        DateTo like @searchKey or 
                                                        DateCreated like @searchKey or 
                                                        DateModified like @searchKey or 
                                                        CreatedBy like @searchKey or 
                                                        ModifiedBy like @searchKey 
                                                        limit @pageItem offset @offset";
                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("offset", offset);
                return await db.QueryAsync<ConfigModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ConfigExists(ConfigModel config)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select configName from config where 
                                                configType = @configType and 
                                                configName = @configName";

                var param = new DynamicParameters();
                param.Add("@configType", config.ConfigType);
                param.Add("@configName", config.ConfigName);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IEnumerable<ConfigModel>> GetConfigUOMs()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"SELECT configType, 
                                            configName, 
                                            description 
                                    FROM config 
                                    ORDER BY intValue";

                return await db.QueryAsync<ConfigModel>(strQry, commandType: CommandType.Text);
            }
        }

        public async Task<ConfigModel> GetConfigById(string configType, string configName)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"select * from config where configType = @configType and 
                                                            configName = @configName";
                var param = new DynamicParameters();
                param.Add("@configType", configType);
                param.Add("@configName", configName);
                var res = await db.QuerySingleOrDefaultAsync<ConfigModel>(strQry, param);

                return await db.QuerySingleOrDefaultAsync<ConfigModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<bool> ConfigInUse(string configType)
        {
            string strQry = string.Empty;

            // query build
            switch (configType.ToUpper())
            {
                case "UOM":
                    strQry = "select count(sku) from product where uomRef = @configType";
                    break;

                default:
                    // return false; do nothing
                    break;
            }

            if (strQry.Length > 0)
            {
                using (IDbConnection db = new MySqlConnection(ConnString))
                {
                    db.Open();

                    var param = new DynamicParameters();
                    param.Add("@configType", configType);

                    var res = await db.ExecuteScalarAsync<int>(strQry, param);
                    if (res == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> CreateConfig(ConfigModel config)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"insert into config(configType, 
                                                configName, 
                                                description, 
                                                stringValue, 
                                                intValue, 
                                                dateFrom, 
                                                dateTo, 
                                                createdBy, 
                                                modifiedBy) 
                                            values(@configType, 
                                                    @configName, 
                                                    @description, 
                                                    @stringValue, 
                                                    @intValue, 
                                                    @dateFrom, 
                                                    @dateTo, 
                                                    @createdBy, 
                                                    @modifiedBy)";

                int res = await db.ExecuteAsync(strQry, config);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(config);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> Updateconfig(ConfigModel config)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = @"update config set description = @description, 
                                                stringValue = @stringValue,
                                                intValue = @intValue,
                                                dateFrom = @dateFrom,
                                                dateTo = @dateTo,
                                                modifiedBy = @modifiedBy where 
                                                configType = @configType and 
                                                configName = @configName";

                int res = await db.ExecuteAsync(strQry, config);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(config);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> Deleteconfig(string configType, string configName, string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                var config = await GetConfigById(configType, configName);

                if (config != null)
                {
                    string strQry = @"delete from config where configType = @configType and 
                                                            configName = @configName";
                    var param = new DynamicParameters();
                    param.Add("@configType", configType);
                    param.Add("@configName", configName);
                    int res = await db.ExecuteAsync(strQry, param);

                    if (res > 0)
                    {
                        // log audit
                        var audit = await AuditBuilder.BuildTranAuditDEL(config, userAccountId);

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
