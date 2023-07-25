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

namespace aims_api.Repositories.Implementation
{
    public class InvAdjustUserFieldRepository : IInvAdjustUserFieldRepository
    {
        private string ConnString;
        private string DatabaseName;
        IAuditTrailRepository AuditTrailRepo;
        InvAdjustUserFieldAudit AuditBuilder;
        public InvAdjustUserFieldRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            DatabaseName = tenantProvider.GetTenant().DBName;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new InvAdjustUserFieldAudit();
        }

        public async Task<bool> ChkColExists(string fieldName)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT count(COLUMN_NAME) inUse
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND 
		                                    TABLE_NAME = 'pouserfield' AND 
		                                    COLUMN_NAME = @fieldName;";

                var param = new DynamicParameters();
                param.Add("@dbName", DatabaseName);
                param.Add("@fieldName", fieldName);

                var res = await db.ExecuteScalarAsync<int>(strQry, param);

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CreateInvAdjustUField(string fieldName, string createdBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"ALTER TABLE invadjustuserfield ADD `{fieldName}` VARCHAR(255) NULL;";

                var res = await db.ExecuteAsync(strQry);

                // check column if created successfully
                var isExists = await ChkColExists(fieldName);

                if (isExists)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(fieldName, createdBy);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> CreateInvAdjustUserField(InvAdjustUserFieldModel invAdjustUserField)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into InvAdjustUserField(invAdjustId)
 												values(@invAdjustId)";

                int res = await db.ExecuteAsync(strQry, invAdjustUserField);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvAdjustUField(string fieldName, string userAccountId)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"alter table invadjustuserfield drop `{fieldName}`";
                int res = await db.ExecuteAsync(strQry);

                var isExists = await ChkColExists(fieldName);

                if (!isExists)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditDEL(fieldName, userAccountId);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteInvAdjustUserField(string invAdjustId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from InvAdjustUserField where 
														invAdjustId = @invAdjustId";
                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);
                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<dynamic?> GetInvAdjustUFields()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT COLUMN_NAME ColumnName
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND 
		                                    TABLE_NAME = 'invadjustuserfield'";

                var param = new DynamicParameters();
                param.Add("@dbName", DatabaseName);

                var res = await db.QueryAsync<UserFieldModel>(strQry, param);

                if (res.Any())
                {
                    var expando = new Dictionary<string, object?>();

                    foreach (var itm in res)
                    {
                        expando.Add(itm.ColumnName, null);
                    }

                    return expando;
                }
            }

            return null;
        }

        public async Task<dynamic> GetInvAdjustUserFieldById(string invAdjustId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustUserField where 
														invAdjustId = @invAdjustId";

                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);
                return await db.QuerySingleOrDefaultAsync<dynamic>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvAdjustUserFieldModel>> GetInvAdjustUserFieldPg(int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = "select * from InvAdjustUserField limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustUserFieldModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<IEnumerable<InvAdjustUserFieldModel>> GetInvAdjustUserFieldPgSrch(string searchKey, int pageNum, int pageItem)
        {
            // pagination setup
            int offset = (pageNum - 1) * pageItem;
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from InvAdjustUserField where 
														poId like @searchKey 
														limit @pageItem offset @offset";

                var param = new DynamicParameters();
                param.Add("@searchKey", $"%{searchKey}%");
                param.Add("@pageItem", pageItem);
                param.Add("@offset", offset);
                return await db.QueryAsync<InvAdjustUserFieldModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<string?> GetInvAdjustUsrFldQryFilter()
        {
            // get wildcard search filter script of product user fields
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"with tmpData  AS (SELECT group_concat(CONCAT('prodUF.', COLUMN_NAME)) ColumnName 
                                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                                    WHERE TABLE_SCHEMA = '@dbName' AND 
			                                                    TABLE_NAME = 'invadjustuserfield') 
                                                    SELECT CONCAT(REPLACE(ColumnName, "","", "" like @searchKey or ""), ' like @searchKey ') 
                                                    FROM tmpData";

                var param = new DynamicParameters();
                param.Add("@dbName", DatabaseName);

                var res = await db.ExecuteScalarAsync<string>(strQry, param);

                if (res != null && res.Length > 0)
                {
                    return res;
                }
            }

            return null;
        }

        public async Task<bool> InitInvAdjustUField(IDbConnection db, string invAdjustId)
        {
            string strQry = $"insert into invadjustuserfield(invAdjustId) values(@invAdjustId);";

            var param = new DynamicParameters();
            param.Add("@invAdjustId", invAdjustId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> InvAdjustUserFieldExists(string invAdjustId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select invAdjustId from InvAdjustUserField where 
														invAdjustId = @invAdjustId";

                var param = new DynamicParameters();
                param.Add("@invAdjustId", invAdjustId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustUField(IDbConnection db, string invAdjustId, string createdBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update invadjustuserfield set {qryCols} where invAdjustId = @invAdjustId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADDDyn(invAdjustId, createdBy, data);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"ALTER TABLE invadjustuserfield RENAME COLUMN `{oldFieldName}` TO `{newFieldName}`;";

                int res = await db.ExecuteAsync(strQry);

                bool newExists = await ChkColExists(newFieldName);
                bool oldExists = await ChkColExists(oldFieldName);

                if (newExists && !oldExists)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(oldFieldName, newFieldName, modifiedBy);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustUFieldMOD(IDbConnection db, string invAdjustId, string modifiedBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update invadjustuserfield set {qryCols} where invAdjustId = @invAdjustId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMODDyn(invAdjustId, modifiedBy, data);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateInvAdjustUserField(InvAdjustUserFieldModel invAdjustUserField)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update InvAdjustUserField set  where 
														invAdjustId = @invAdjustId";

                int res = await db.ExecuteAsync(strQry, invAdjustUserField);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
