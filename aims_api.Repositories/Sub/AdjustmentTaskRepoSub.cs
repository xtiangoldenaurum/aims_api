using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Sub
{
    public class AdjustmentTaskRepoSub
    {
        IAuditTrailRepository AuditTrailRepo;
        InvAdjustAudit AuditBuilder;
        public AdjustmentTaskRepoSub(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new InvAdjustAudit();
        }
        public async Task<bool> HasPendingAdjustmentTask(IDbConnection db, string invAdjustId)
        {
            string strQry = @"CALL `spCountInvAdjustPendingAdjustment`(@currInvAdjustId);";

            var param = new DynamicParameters();
            param.Add("@currInvAdjustId", invAdjustId);

            int res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<string?> DefineTranTypeByDocId(IDbConnection db, string docLineId)
        {
            string strQry = @"CALL `spDefineDocTranOrigin`(@docLineId)";

            var param = new DynamicParameters();
            param.Add("@docLineId", docLineId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }
    }
}
