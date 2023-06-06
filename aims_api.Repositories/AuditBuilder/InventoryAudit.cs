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
    internal class InventoryAudit
    {
        public async Task<AuditTrailModel> BuildInvAuditADD(InventoryModel inv, string userAccountId, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(inv);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = inv.InventoryId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(InventoryModel inv, string userAccountId, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(inv);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = inv.InventoryId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(string invId, string invStatus, string userAccountId, TranType tranTyp)
        {
            var inv = new InventoryModel()
            {
                InventoryId = invId,
                InventoryStatusId = invStatus,
            };

            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(inv);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = inv.InventoryId,
                    Data = data,
                    Remarks = "Inventory Status updated."
                };
            });
        }

    }
}
