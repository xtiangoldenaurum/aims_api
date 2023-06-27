using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.AuditBuilder;
using aims_api.Repositories.Interface;
using aims_api.Utilities.Interface;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class InvMoveRepository : IInvMoveRepository
    {
        private string ConnString;
        IIdNumberRepository IdNumberRepo;
        IAuditTrailRepository AuditTrailRepo;
		InvMoveAudit AuditBuilder;
		IPagingRepository PagingRepo;

		public InvMoveRepository(ITenantProvider tenantProvider,
                            IAuditTrailRepository auditTrailRepo,
                            IIdNumberRepository idNumberRepo)
        {
            ConnString = tenantProvider.GetTenant().SqlConnectionString;
            AuditTrailRepo = auditTrailRepo;
            IdNumberRepo = idNumberRepo;
			PagingRepo = new PagingRepository();
			AuditBuilder = new InvMoveAudit();
		}
        public Task<InvMoveCreateTranResult> CreateInvMoveMod(InvMoveModelMod invMove)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> CreateInvMove(IDbConnection db, InvMoveModel invMove)
        {
			// define po status
			invMove.InvMoveStatusId = (InvMoveStatus.CREATED).ToString();

			string strQry = @"insert into invmove(invMoveId, 
														invMoveStatusId, 
														warehouseId, 
														reasonCodeId, 
														createdBy, 
														modifiedBy, 
														remarks)
 												values(@invMoveId, 
														@invMoveStatusId, 
														@warehouseId, 
														@reasonCodeId, 
														@createdBy, 
														@modifiedBy, 
														@remarks)";

			int res = await db.ExecuteAsync(strQry, invMove);

			if (res > 0)
			{
				// log audit
				var audit = await AuditBuilder.BuildTranAuditADD(invMove, TranType.INVMOV);

				if (await AuditTrailRepo.CreateAuditTrail(db, audit))
				{
					return true;
				}
			}

			return false;
		}
    }
}
