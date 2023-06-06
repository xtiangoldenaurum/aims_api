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
    internal class ReturnsDetailAudit
    {
        public async Task<AuditTrailModel> BuildPODtlAuditADD(ReturnsDetailModel retDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(retDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = retDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = retDetail.ReturnsLineId,
                    Data = data,
                    Remarks = "Returns Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildPODtlAuditMOD(ReturnsDetailModel retDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(retDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = retDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = retDetail.ReturnsLineId,
                    Data = data,
                    Remarks = "Returns Detail Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(ReturnsDetailModel retDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(retDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = retDetail.ReturnsLineId,
                    Data = data,
                    Remarks = "Returns Detail Deleted"
                };
            });
        }
    }
}
