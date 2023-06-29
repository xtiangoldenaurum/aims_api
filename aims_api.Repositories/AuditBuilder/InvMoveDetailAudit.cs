using aims_api.Enums;
using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    public class InvMoveDetailAudit
    {
        public async Task<AuditTrailModel> BuildPODtlAuditADD(InvMoveDetailModel invMoveDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(invMoveDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invMoveDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invMoveDetail.InvMoveLineId,
                    Data = data,
                    Remarks = "Movement Document Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildPODtlAuditMOD(InvMoveDetailModel invMoveDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(invMoveDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invMoveDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invMoveDetail.InvMoveLineId,
                    Data = data,
                    Remarks = "Movement Document Detail Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(InvMoveDetailModel invMoveDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(invMoveDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invMoveDetail.InvMoveLineId,
                    Data = data,
                    Remarks = "Movement Document Detail Deleted"
                };
            });
        }
    }
}
