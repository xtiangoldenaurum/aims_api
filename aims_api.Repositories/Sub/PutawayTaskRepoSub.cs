using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
  subtitute classes are clever way to resolve circular dependencies,
  issues where Class A requires method from Class B and vice versa
  JADC 12-06-2022
*/

namespace aims_api.Repositories.Sub
{
    public class PutawayTaskRepoSub
    {
        IAuditTrailRepository AuditTrailRepo;
        POAudit AuditBuilder;

        public PutawayTaskRepoSub(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new POAudit();
        }

        public async Task<bool> HasPendingPutawayTaskPO(IDbConnection db, string poId)
        {
            string strQry = @"CALL `spCountPOPendingPutaway`(@currPOId);";

            var param = new DynamicParameters();
            param.Add("@currPOId", poId);

            int res = await db.ExecuteScalarAsync<int>(strQry, param);

            if (res == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HasPendingPutawayTaskRet(IDbConnection db, string returnsId)
        {
            string strQry = @"CALL `spCountRetPendingPutaway`(@currRetId);";

            var param = new DynamicParameters();
            param.Add("@currRetId", returnsId);

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
