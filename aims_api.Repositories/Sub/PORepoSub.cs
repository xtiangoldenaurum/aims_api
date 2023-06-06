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
    public class PORepoSub
    {
        IAuditTrailRepository AuditTrailRepo;
        POAudit AuditBuilder;

        public PORepoSub(IAuditTrailRepository auditTrailRepo)
        {
            AuditTrailRepo = auditTrailRepo;
            AuditBuilder = new POAudit();
        }

        public async Task<POModel> LockPO(IDbConnection db, string poId)
        {
            string strQry = @"SELECT * FROM po WHERE poId = @poId FOR UPDATE;";

            var param = new DynamicParameters();
            param.Add("@poId", poId);

            return await db.QuerySingleOrDefaultAsync<POModel>(strQry, param);
        }

        public async Task<string?> GetPoUpdatedStatus(IDbConnection db, string poId)
        {
            string strQry = @"call `spGetPOUpdatedStatus`(@paramPOId);";

            var param = new DynamicParameters();
            param.Add("@paramPOId", poId);

            return await db.ExecuteScalarAsync<string?>(strQry, param);
        }

        public async Task<bool> UpdatePO(IDbConnection db, POModel po, TranType tranTyp)
        {
            string strQry = @"update PO set 
							                refNumber = @refNumber, 
							                refNumber2 = @refNumber2, 
							                supplierId = @supplierId, 
							                supplierName = @supplierName, 
							                supplierAddress = @supplierAddress, 
							                supplierContact = @supplierContact, 
							                supplierEmail = @supplierEmail, 
							                carrierId = @carrierId, 
							                carrierName = @carrierName, 
							                carrierAddress = @carrierAddress, 
							                carrierContact = @carrierContact, 
							                carrierEmail = @carrierEmail, 
							                orderDate = @orderDate, 
							                arrivalDate = @arrivalDate, 
							                arrivalDate2 = @arrivalDate2, 
							                poStatusId = @poStatusId, 
							                modifiedBy = @modifiedBy, 
							                remarks = @remarks where 
							                poId = @poId";

            int res = await db.ExecuteAsync(strQry, po);

            if (res > 0)
            {
                // log audit
                var audit = await AuditBuilder.BuildTranAuditMOD(po, tranTyp);

                if (await AuditTrailRepo.CreateAuditTrail(db, audit))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
