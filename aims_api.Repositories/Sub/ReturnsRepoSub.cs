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
    public class ReturnsRepoSub
    {
        IAuditTrailRepository AuditTrailRepo;
        ReturnsAudit AuditBuilder;

        public ReturnsRepoSub(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ReturnsAudit();
        }

        public async Task<ReturnsModel> LockReturns(IDbConnection db, string returnsId)
        {
            string strQry = @"SELECT * FROM returns WHERE returnsId = @returnsId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@returnsId", returnsId);

            return await db.QuerySingleOrDefaultAsync<ReturnsModel>(strQry, param);
        }

        public async Task<string?> GetReturnsUpdatedStatus(IDbConnection db, string returnsId)
        {
            string strQry = @"call `spGetReturnsUpdatedStatus`(@paramReturnsId);";

            var param = new DynamicParameters();
            param.Add("@paramReturnsId", returnsId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<bool> UpdateReturns(IDbConnection db, ReturnsModel returns, TranType tranTyp)
        {
            string strQry = @"update Returns set 
									            refNumber = @refNumber, 
									            refNumber2 = @refNumber2, 
									            storeFrom = @storeFrom, 
									            storeAddress = @storeAddress, 
									            storeContact = @storeContact, 
									            storeEmail = @storeEmail, 
									            arrivalDate = @arrivalDate, 
									            arrivalDate2 = @arrivalDate2, 
									            returnsStatusId = @returnsStatusId, 
									            modifiedBy = @modifiedBy, 
									            remarks = @remarks where 
									            returnsId = @returnsId";

            int res = await db.ExecuteAsync(strQry, returns);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(returns, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
