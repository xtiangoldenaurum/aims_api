using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class LocationGroupAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(LocationGroupModel locationGroup)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(locationGroup);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = locationGroup.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = locationGroup.LocationGroupId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(LocationGroupModel locationGroup)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(locationGroup);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = locationGroup.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = locationGroup.LocationGroupId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(LocationGroupModel locationGroup, string userAccountId)
        {
            string actTyp = "DEL";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(locationGroup);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = locationGroup.LocationGroupId,
                    Data = data
                };
            });
        }
    }
}
