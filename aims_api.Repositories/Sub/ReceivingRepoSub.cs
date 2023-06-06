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
    - simply replicates required methods from main repo class
    JADC 12-06-2022
*/

namespace aims_api.Repositories.Sub
{
    public class ReceivingRepoSub
    {
        IAuditTrailRepository AuditTrailRepo;
        ReceivingAudit AuditBuilder;

        public ReceivingRepoSub(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new ReceivingAudit();
        }

        public async Task<ReceivingModel> LockReceiveDetailRefMulti(IDbConnection db, string docLineId, string inventoryId)
        {
            // hold currentr eceive transaction
            string strQry = @"select * 
                                from receiving 
                                where docLineId = @docLineId and 
                                        inventoryId = @inventoryId and 
                                        seqnum = 1 and 
                                        receivingStatusId = 'CREATED' 
                                for update;";

            var param = new DynamicParameters();
            param.Add("@docLineId", docLineId);
            param.Add("@inventoryId", inventoryId);

            return await db.QuerySingleOrDefaultAsync<ReceivingModel>(strQry, param);
        }

        public async Task<ReceivingModel> GetReceivingByIdMod(IDbConnection db, string receivingId)
        {
            string strQry = @"select * from Receiving where 
														receivingId = @receivingId";

            var param = new DynamicParameters();
            param.Add("@receivingId", receivingId);

            return await db.QuerySingleOrDefaultAsync<ReceivingModel>(strQry, param, commandType: CommandType.Text);
        }

        public async Task<bool> SetReceivingStatus(IDbConnection db, string receivingId, string receivingStatus, TranType tranTyp, string userAccountId)
        {
            string strQry = @"update receiving set receivingStatusId = @receivingStatusId, 
                                                    modifiedBy = @modifiedBy
                                    where receivingId = @receivingId;";

            var param = new DynamicParameters();
            param.Add("@receivingStatusId", receivingStatus);
            param.Add("@modifiedBy", userAccountId);
            param.Add("@receivingId", receivingId);

            int res = await db.ExecuteAsync(strQry, param);

            if (res > 0)
            {
                // get updated receiving record
                var rcving = await GetReceivingByIdMod(db, receivingId);

                if (rcving != null)
                {
                    // log audit
                    var audit = await AuditBuilder.BuildTranAuditMOD(rcving, tranTyp);

                    if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> CreateReceivingMod(IDbConnection db, ReceivingModel receiving, TranType tranTyp)
        {
            string strQry = @"insert into Receiving(receivingId, 
														docLineId, 
														inventoryId, 
														seqNum, 
                                                        receivingStatusId, 
														createdBy, 
														modifiedBy)
 												values(@receivingId, 
														@docLineId, 
														@inventoryId, 
														@seqNum, 
                                                        @receivingStatusId, 
														@createdBy, 
														@modifiedBy)";

            int res = await db.ExecuteAsync(strQry, receiving);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildReceivingAuditADD(receiving, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
