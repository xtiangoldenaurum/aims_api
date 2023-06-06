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
    public class UsrAccUserFieldRepository : IUsrAccUserFieldRepository
    {
        private string ConnString;
        private string DatabaseName;
        IAuditTrailRepository AuditTrailRepo;
        UserAccUserFieldAudit AuditBuilder;

        public UsrAccUserFieldRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            DatabaseName = tenantProvider.GetTenant().DBName;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new UserAccUserFieldAudit();
        }

        public async Task<SOUserFieldModel> GetAccUserFieldById(string userAccountId)
        {
            // pagination setup
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select * from useraccuserfields where 
														userAccountId = @userAccountId";

                var param = new DynamicParameters();
				param.Add("@userAccountId", userAccountId);
                return await db.QuerySingleOrDefaultAsync<SOUserFieldModel>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<dynamic?> GetUserAccountUFields()
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"SELECT COLUMN_NAME ColumnName
                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                    WHERE TABLE_SCHEMA = @dbName AND 
		                                    TABLE_NAME = 'useraccuserfields'";

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

        public async Task<string?> GetAccUsrFldQryFilter()
        {
            // get wildcard search filter script of product user fields
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"with tmpData  AS (SELECT group_concat(CONCAT('prodUF.', COLUMN_NAME)) ColumnName 
                                                    FROM INFORMATION_SCHEMA.COLUMNS 
                                                    WHERE TABLE_SCHEMA = '@dbName' AND 
			                                                    TABLE_NAME = 'useraccuserfields' AND 
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

        public async Task<bool> CreateUserAccUField(string fieldName, string createdBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"ALTER TABLE useraccuserfields ADD `{fieldName}` VARCHAR(255) NULL;";

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

        public async Task<bool> InitUserAccUField(IDbConnection db, string userAccountId)
        {
            string strQry = $"insert into useraccuserfields(userAccountId) values(@userAccountId);";

            var param = new DynamicParameters();
            param.Add("@userAccountId", userAccountId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateUserAccUField(IDbConnection db, string userAccountId, string createdBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update useraccuserfields set {qryCols} where userAccountId = @userAccountId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditADD(userAccountId, createdBy, data);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> UpdateUserAccUFieldMOD(IDbConnection db, string userAccountId, string modifiedBy, dynamic data)
        {
            // default sku insert then fields update from null will be executed
            var qryHelper = new QryHelper();
            var qryCols = await qryHelper.GetUpdateQry(data);

            if (qryCols != null)
            {
                string strQry = $"update useraccuserfields set {qryCols} where userAccountId = @userAccountId;";

                var param = (DynamicParameters?)await qryHelper.GetParams(data);

                int res = await db.ExecuteAsync(strQry, param);

                if (res > 0)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(userAccountId, modifiedBy, data);

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
		                                    TABLE_NAME = 'useraccuserfields' AND 
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
        public async Task<bool> UpdateUserAccUField(string oldFieldName, string newFieldName, string modifiedBy)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"ALTER TABLE useraccuserfields RENAME COLUMN `{oldFieldName}` TO `{newFieldName}`;";

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

        public async Task<bool> DeleteUserAccUField(string fieldName, string userAccountId)
        {
            // note: MySql does not support rollback on database table schema alter queries
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = $"alter table useraccuserfields drop `{fieldName}`";
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

        public async Task<bool> UsrAccUserFieldExists(string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();
                string strQry = @"select soId from useraccuserfields where 
														userAccountId = @userAccountId";

                var param = new DynamicParameters();
				param.Add("@userAccountId", userAccountId);

                var res = await db.ExecuteScalarAsync(strQry, param);
                if (res != null && res != DBNull.Value)
                {
                    return true;
                }
            }

            return false;
        }

		// place here InUse checker function

        public async Task<bool> DeleteUserAccUField(string userAccountId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                string strQry = @"delete from SOUserField where 
														userAccountId = @userAccountId";
                var param = new DynamicParameters();
				param.Add("@soId", userAccountId);
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
