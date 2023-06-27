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
    internal class InvMoveAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(InvMoveModel invMove, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(invMove);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invMove.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invMove.InvMoveId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(InvMoveModel invMove, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(invMove);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = invMove.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invMove.InvMoveId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(InvMoveModel invMove, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(invMove);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = invMove.InvMoveId,
                    Data = data
                };
            });
        }
    }
}
