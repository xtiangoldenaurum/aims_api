using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class AccessRightAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(AccessRightModel accessRight)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(accessRight);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = accessRight.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = accessRight.AccessRightId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDetailADD(AccessRightModel accessRight, IEnumerable<AccessRightDetailModelMod> accessRightDetail)
        {
            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(accessRightDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = accessRight.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = accessRight.AccessRightId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(AccessRightModel accessRight)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(accessRight);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = accessRight.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = accessRight.AccessRightId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDetailMOD(AccessRightModel accessRight, IEnumerable<AccessRightDetailModelMod> accessRightDetail)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(accessRight);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = accessRight.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = accessRight.AccessRightId,
                    Data = data
                };
            });
        }
    }
}
