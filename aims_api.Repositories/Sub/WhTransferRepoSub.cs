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
    public class WhTransferRepoSub
    {
        IAuditTrailRepository AuditTrailRepo;
        WhTransferAudit AuditBuilder;

        public WhTransferRepoSub(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new WhTransferAudit();
        }

        public async Task<WhTransferModel> LockWhTransfer(IDbConnection db, string whTransId)
        {
            string strQry = @"SELECT * FROM whtransfer WHERE whTransferId = @whTransId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@whTransId", whTransId);

            return await db.QuerySingleOrDefaultAsync<WhTransferModel>(strQry, param);
        }

        public async Task<string?> GetReturnsUpdatedStatus(IDbConnection db, string whTransId)
        {
            string strQry = @"call `spGetWhTransUpdatedStatus`(@paramWhTransId);";

            var param = new DynamicParameters();
            param.Add("@paramWhTransId", whTransId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<bool> UpdateWhTransfer(IDbConnection db, WhTransferModel whTransfer, TranType tranType)
        {
            string strQry = @"update WhTransfer set 
							                        refNumber = @refNumber, 
							                        whFrom = @whFrom, 
							                        whFromAddress = @whFromAddress, 
							                        whFromContact = @whFromContact, 
							                        whFromEmail = @whFromEmail, 
							                        arrivalDate = @arrivalDate, 
							                        arrivalDate2 = @arrivalDate2, 
							                        whTransStatusId = @whTransStatusId, 
							                        modifiedBy = @modifiedBy, 
							                        remarks = @remarks where 
							                        whTransferId = @whTransferId";

            int res = await db.ExecuteAsync(strQry, whTransfer);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildWhTransAuditMOD(whTransfer, tranType);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
