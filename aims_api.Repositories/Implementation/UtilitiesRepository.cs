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
    public class UtilitiesRepository : IUtilitiesRepository
    {
        private string ConnString;
        ITenantProvider TenantProvider;
        IAuditTrailRepository AuditTrailRepo;
        ConfigAudit AuditBuilder;

        public UtilitiesRepository(ITenantProvider tenantProvider, IAuditTrailRepository auditTrailRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            TenantProvider = tenantProvider;
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ConfigAudit();
        }

        public async Task<string?> DefineTranTypeByDocId(string documentId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"CALL `spDefineDocTranOriginById`(@docId)";

                var param = new DynamicParameters();
                param.Add("@docId", documentId);

                return await db.ExecuteScalarAsync<string?>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<string?> DefineTranTypeByDocLineId(string docLineId)
        {
            using (IDbConnection db = new MySqlConnection(ConnString))
            {
                db.Open();

                string strQry = @"CALL `spDefineDocTranOriginByLineId`(@docLineId)";

                var param = new DynamicParameters();
                param.Add("@docLineId", docLineId);

                return await db.ExecuteScalarAsync<string?>(strQry, param, commandType: CommandType.Text);
            }
        }

        public async Task<Tenant?> GetTenantSettings()
        {
            return await Task.Run(() =>
            {
                return TenantProvider.GetTenant();
            });
        }

    }
}
