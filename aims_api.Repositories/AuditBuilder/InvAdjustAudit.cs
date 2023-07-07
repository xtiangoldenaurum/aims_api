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
    public class InvAdjustAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(InvAdjustModel invAdjust, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(invAdjust);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invAdjust.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invAdjust.InvAdjustId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(InvAdjustModel invAdjust, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(invAdjust);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invAdjust.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invAdjust.InvAdjustId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(InvAdjustModel invAdjust, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(invAdjust);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invAdjust.InvAdjustId,
                    Data = data
                };
            });
        }
    }
}
