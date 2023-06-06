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
    public class UniqueTagsAudit
    {
        public async Task<AuditTrailModel> BuildUniqTagsAuditADD(UniqueTagsModel uniqTag, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(uniqTag);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = uniqTag.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = $"UniqueTagID-{uniqTag.UniqueTagId}",
                    Data = data,
                    Remarks = "Unique Tag Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(UniqueTagsModel uniqTags, string notedRecId, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(uniqTags);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = notedRecId,
                    Data = data,
                    Remarks = "Unique Tags Deleted"
                };
            });
        }
    }
}
