using aims_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aims_api.Repositories.AuditBuilder
{
    internal class UserAccountAudit
    {
        public async Task<AuditTrailModel> BuildTranAuditADD(UserAccountModel userAccount)
        {
            // remove image data
            userAccount.Image = null;

            string actTyp = "ADD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(userAccount);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccount.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = userAccount.UserAccountId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditMOD(UserAccountModel userAccount)
        {
            // remove image data
            userAccount.Image = null;

            string actTyp = "MOD";
            string tranTyp = "CONFIG";
            string data = JsonSerializer.Serialize(userAccount);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccount.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = userAccount.UserAccountId,
                    Data = data
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDeAct(string delUserAccountId, string userAccountId)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = delUserAccountId,
                    Remarks = "Deactivates User account."
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditReAct(string delUserAccountId, string userAccountId)
        {
            string actTyp = "MOD";
            string tranTyp = "CONFIG";

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = tranTyp,
                    RecordId = delUserAccountId,
                    Remarks = "Reactivates User account."
                };
            });
        }
    }
}
