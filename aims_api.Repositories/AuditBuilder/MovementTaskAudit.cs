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
    public class MovementTaskAudit
    {
        public async Task<AuditTrailModel> BuildMovementTaskAuditADD(MovementTaskModel movementTask, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(movementTask);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = movementTask.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = movementTask.MovementTaskId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(MovementTaskModel movementTask, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(movementTask);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = movementTask.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = movementTask.MovementTaskId,
                    Data = data
                };
            });
        }
    }
}
