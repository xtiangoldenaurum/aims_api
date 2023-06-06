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
    internal class InventoryHistAudit
    {
        public async Task<AuditTrailModel> BuildInvHistAuditADD(InventoryHistoryModel invHist, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(invHist);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invHist.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = $"InvId: {invHist.InventoryId}, SeqNum: {invHist.SeqNum}",
                    Data = data
                };
            });
        }

    }
}
