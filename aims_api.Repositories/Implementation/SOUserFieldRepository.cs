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
    public class SOUserFieldRepository : ISOUserFieldRepository
    {
        private string ConnString;
        private string DatabaseName;
        IAuditTrailRepository AuditTrailRepo;
        SOUserFieldAudit AuditBuilder;

        public SOUserFieldRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            DatabaseName = tenantProvider.GetTenant().DBName;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new SOUserFieldAudit();
        }

        public async Task<dynamic> GetSOUserFieldById(string soId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from SOUserField where 
														soId = @soId";

                var param = new DynamicParameters();
				param.Add("@soId", soId);
                return await db.QuerySingleOrDefaultAsync<dynamic>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<dynamic?> GetSOUFields()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT COLUMN_NAME ColumnName
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND 
		                                    TABLE_NAME = 'souserfield'";

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

        public async Task<string?> GetSOUsrFldQryFilter()
        {
            // get wildcard search filter script of product user fields
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"with tmpData  AS (SELECT group_concat(CONCAT('prodUF.', COLUMN_NAME)) ColumnName 
                                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                                    WHERE TABLE_SCHEMA = '@dbName' AND 
			                                                    TABLE_NAME = 'souserfield' AND 
			                                                    COLUMN_NAME <> 'sku') 
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

        public async Task<bool> CreateSOUField(string fieldName, string createdBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"ALTER TABLE souserfield ADD `{fieldName}` VARCHAR(255) NULL;";

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

        public async Task<bool> InitSOUField(IDbConnection db, string soId)
        {
            string strQry = $"insert into souserfield(soId) values(@soId);";

            var param = new DynamicParameters();
            param.Add("@soId", soId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateSOUField(IDbConnection db, string soId, string createdBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update souserfield set {qryCols} where soId = @soId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADDDyn(soId, createdBy, data);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateSOUFieldMOD(IDbConnection db, string soId, string modifiedBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update souserfield set {qryCols} where soId = @soId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMODDyn(soId, modifiedBy, data);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> ChkColExists(string fieldName)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT count(COLUMN_NAME) inUse
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND 
		                                    TABLE_NAME = 'souserfield' AND 
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
        public async Task<bool> UpdateSOUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = $"ALTER TABLE souserfield RENAME COLUMN `{oldFieldName}` TO `{newFieldName}`;";

                int res = await db.ExecuteAsync(strQry, tran);

                bool newExists = await ChkColExists(newFieldName);
                bool oldExists = await ChkColExists(oldFieldName);

                if (newExists && !oldExists)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(oldFieldName, newFieldName, modifiedBy);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DeleteSOUField(string fieldName, string userAccountId)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = $"alter table souserfield drop `{fieldName}`";
                int res = await db.ExecuteAsync(strQry, tran);

                var isExists = await ChkColExists(fieldName);

                if (!isExists)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditDEL(fieldName, userAccountId);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> SOUserFieldExists(string soId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soId from SOUserField where 
														soId = @soId";

                var param = new DynamicParameters();
				param.Add("@soId", soId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CreateSOUserField(SOUserFieldModel soUserField)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"insert into SOUserField(soId)
 												values(@soId)";

                int res = await db.ExecuteAsync(strQry, soUserField);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateSOUserField(SOUserFieldModel soUserField)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"update SOUserField set  where 
														soId = @soId";

                int res = await db.ExecuteAsync(strQry, soUserField);

                if (res > 0)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteSOUserField(string soId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SOUserField where 
														soId = @soId";
                var param = new DynamicParameters();
				param.Add("@soId", soId);
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
