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
    public class InvCountDetailAudit
    {
        public async Task<AuditTrailModel> BuildInvCountDtlAuditADD(InvCountDetailModel invCountDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(invCountDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invCountDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invCountDetail.InvCountLineId,
                    Data = data,
                    Remarks = "Count Document Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildInvCountDtlAuditMOD(InvCountDetailModel invCountDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(invCountDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invCountDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invCountDetail.InvCountLineId,
                    Data = data,
                    Remarks = "Count Document Detail Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(InvCountDetailModel invCountDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(invCountDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invCountDetail.InvCountLineId,
                    Data = data,
                    Remarks = "Count Document Detail Deleted"
                };
            });
        }
    }
}
