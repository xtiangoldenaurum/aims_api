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
    internal class ReceivingAudit
    {
        public async Task<AuditTrailModel> BuildReceivingAuditADD(ReceivingModel receiving, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(receiving);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = receiving.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = receiving.ReceivingId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(ReceivingModel receiving, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(receiving);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = receiving.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = receiving.ReceivingId,
                    Data = data
                };
            });
        }

    }
}
