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
    internal class PODetailAudit
    {
        public async Task<AuditTrailModel> BuildPODtlAuditADD(PODetailModel poDetail, TranType tranTyp)
        {
            string actTyp = "ADD";
            string data = JsonSerializer.Serialize(poDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = poDetail.CreatedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = poDetail.PoLineId,
                    Data = data,
                    Remarks = "PO Detail Created"
                };
            });
        }

        public async Task<AuditTrailModel> BuildPODtlAuditMOD(PODetailModel poDetail, TranType tranTyp)
        {
            string actTyp = "MOD";
            string data = JsonSerializer.Serialize(poDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = poDetail.ModifiedBy,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = poDetail.PoLineId,
                    Data = data,
                    Remarks = "PO Detail Modified"
                };
            });
        }

        public async Task<AuditTrailModel> BuildTranAuditDEL(PODetailModel poDetail, string userAccountId, TranType tranTyp)
        {
            string actTyp = "DEL";
            string data = JsonSerializer.Serialize(poDetail);

            return await Task.Run(() => {
                return new AuditTrailModel()
                {
                    AuditDate = DateTime.Now,
                    UserAccountId = userAccountId,
                    ActionTypeId = actTyp,
                    TransactionTypeId = (tranTyp).ToString(),
                    RecordId = poDetail.PoLineId,
                    Data = data,
                    Remarks = "PO Detail Deleted"
                };
            });
        }
    }
}
