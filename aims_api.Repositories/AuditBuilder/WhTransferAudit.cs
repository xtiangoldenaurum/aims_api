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
    internal class WhTransferAudit
    {
        public async Task<AuditTrailModel> BuildWhTransAuditADD(WhTransferModel whTrans, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(whTrans);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = whTrans.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = whTrans.WhTransferId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildWhTransAuditMOD(WhTransferModel whTrans, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(whTrans);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = whTrans.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = whTrans.WhTransferId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildWhTransAuditDEL(WhTransferModel whTrans, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(whTrans);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = whTrans.WhTransferId,
                    Data = data
                };
            });
        }
    }
}
