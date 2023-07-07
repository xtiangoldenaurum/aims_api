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
    public class InvAdjustDetailAudit
    {
        public async Task<AuditTrailModel> BuildInvAdjustDtlAuditADD(InvAdjustDetailModel invAdjustDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(invAdjustDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invAdjustDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invAdjustDetail.InvAdjustLineId,
                    Data = data,
                    Remarks = "Adjustment Document Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildInvAdjustDtlAuditMOD(InvAdjustDetailModel invAdjustDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(invAdjustDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invAdjustDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invAdjustDetail.InvAdjustLineId,
                    Data = data,
                    Remarks = "Adjustment Document Detail Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(InvAdjustDetailModel invAdjustDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(invAdjustDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invAdjustDetail.InvAdjustLineId,
                    Data = data,
                    Remarks = "Adjustment Document Detail Deleted"
                };
            });
        }
    }
}
