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
    public class ReferredTagsAudit
    {
        public async Task<AuditTrailModel> BuildRefTagsAuditADD(ReferredTagsModel refTag, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(refTag);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = refTag.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = $"ReferredTagId-{refTag.ReferredTagId}",
                    Data = data,
                    Remarks = "Referred Tag Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(ReferredTagsModel refTag, string notedRecId, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(refTag);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = notedRecId,
                    Data = data,
                    Remarks = "Referred Tag Deleted"
                };
            });
        }
    }
}
