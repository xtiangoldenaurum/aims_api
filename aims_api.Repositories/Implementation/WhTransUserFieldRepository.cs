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
    public class WhTransUserFieldRepository : IWhTransUserFieldRepository
    {
        private string ConnString;
        private string DatabaseName;
        IAuditTrailRepository AuditTrailRepo;
        WhTransUserFieldAudit AuditBuilder;

        public WhTransUserFieldRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            DatabaseName = tenantProvider.GetTenant().DBName;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new WhTransUserFieldAudit();
        }

        public async Task<dynamic> GetWhTransUserFieldById(string whTransferId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from WhTransUserField where 
														whTransferId = @whTransferId";

                var param = new DynamicParameters();
				param.Add("@whTransferId", whTransferId);
                return await db.QuerySingleOrDefaultAsync<dynamic>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<dynamic?> GetWhTransferUFields()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT COLUMN_NAME ColumnName
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND 
		                                    TABLE_NAME = 'whtransuserfield'";

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

        public async Task<string?> GetWhTransUsrFldQryFilter()
        {
            // get wildcard search filter script of product user fields
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"with tmpData  AS (SELECT group_concat(CONCAT('prodUF.', COLUMN_NAME)) ColumnName 
                                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                                    WHERE TABLE_SCHEMA = '@dbName' AND 
			                                                    TABLE_NAME = 'whtransuserfield' AND 
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

        public async Task<bool> CreateWhTransferUField(string fieldName, string createdBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = $"ALTER TABLE whtransuserfield ADD `{fieldName}` VARCHAR(255) NULL;";

                var res = await db.ExecuteAsync(strQry, tran);

                // check column if created successfully
                var isExists = await ChkColExists(fieldName);

                if (isExists)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(fieldName, createdBy);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        tran.Commit();
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> InitWhTransferUField(IDbConnection db, string whTransferId)
        {
            string strQry = $"insert into whtransuserfield(whTransferId) values(@whTransferId);";

            var param = new DynamicParameters();
            param.Add("@whTransferId", whTransferId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateWhTransferUField(IDbConnection db, string whTransferId, string createdBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update whtransuserfield set {qryCols} where whTransferId = @whTransferId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADDDyn(whTransferId, createdBy, data);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateWhTransUFieldMOD(IDbConnection db, string whTransferId, string modifiedBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update whtransuserfield set {qryCols} where whTransferId = @whTransferId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMODDyn(whTransferId, modifiedBy, data);

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
		                                    TABLE_NAME = 'whtransuserfield' AND 
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
        public async Task<bool> UpdateWhTransferUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = $"ALTER TABLE whtransuserfield RENAME COLUMN `{oldFieldName}` TO `{newFieldName}`;";

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

        public async Task<bool> DeleteWhTransferUField(string fieldName, string userAccountId)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                var tran = db.BeginTransaction();

                string strQry = $"alter table whtransuserfield drop `{fieldName}`";
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

        // place here InUse checker function

        public async Task<bool> DeleteWhTransUserField(string whTransferId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from WhTransUserField where 
														whTransferId = @whTransferId";
                var param = new DynamicParameters();
				param.Add("@whTransferId", whTransferId);
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
