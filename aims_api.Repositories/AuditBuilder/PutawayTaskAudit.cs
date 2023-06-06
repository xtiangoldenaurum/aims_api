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
    internal class PutawayTaskAudit
    {
        public async Task<AuditTrailModel> BuildPutawayAuditADD(PutawayTaskModel putaway, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(putaway);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = putaway.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = putaway.PutawayTaskId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(PutawayTaskModel putaway, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(putaway);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = putaway.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = putaway.PutawayTaskId,
                    Data = data
                };
            });
        }

    }
}
